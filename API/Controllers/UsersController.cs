using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs;
using api.Extensions;
using api.Interfaces;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  //All the Users Controller needs authentication
  [Authorize]
  public class UsersController : BaseApiController
  {
    // private readonly DataContext _context;

    private readonly IUserRepository _userRepository;

    public IMapper _mapper { get; }
    private readonly IPhotoService _photoService;

    // public UsersController(DataContext context)
    public UsersController(IUserRepository userRepository,
                            IMapper mapper,
                            IPhotoService photoService)
    {
      _photoService = photoService;
      // _context = context;
      _userRepository = userRepository;
      _mapper = mapper;
    }

    //[AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {

      // var users = await _context.Users.ToListAsync();

      // var users = await _userRepository.GetUsersAsync();
      // var usersToReturn = _mapper.Map<IEnumerable<MemberDto>>(users);
      // return Ok(usersToReturn);

      var members = await _userRepository.GetMembersAsync();
      return Ok(members);

    }

    // api/users/3
    [HttpGet("{username}", Name = "GetUser")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      // var user = await _context.Users.FindAsync(id);

      // var user = await _userRepository.GetUserByUsernameAsync(username);
      // var userToReturn = _mapper.Map<MemberDto>(user);
      // return Ok(userToReturn);

      var member = await _userRepository.GetMemberAsync(username);
      return Ok(member);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
    {
      // var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var username = User.GetUsername();
      var user = await _userRepository.GetUserByUsernameAsync(username);

      _mapper.Map(memberUpdateDto, user);

      _userRepository.Update(user);

      if (await _userRepository.SaveAllAsync()) return NoContent();

      return BadRequest("Failed to update user");
    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

      var result = await _photoService.AddPhotoAsync(file);

      if (result.Error != null) return BadRequest(result.Error.Message);

      var photo = new Photo
      {
        Url = result.SecureUrl.AbsoluteUri,
        PublicId = result.PublicId
      };

      if (user.Photos.Count == 0)
      {
        photo.IsMain = true;
      }

      user.Photos.Add(photo);

      if (await _userRepository.SaveAllAsync())
      {
        // return _mapper.Map<PhotoDto>(photo);
        return CreatedAtRoute("GetUser", new { username = user.UserName }, _mapper.Map<PhotoDto>(photo));
      }

      return BadRequest("Problem Adding Photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if (photo.IsMain) return BadRequest("This is already your main photo");

      var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
      if (currentMain != null) currentMain.IsMain = false;
      photo.IsMain = true;

      if (await _userRepository.SaveAllAsync()) return NoContent();

      return BadRequest("Failed to set main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {

      var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());

      var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

      if (photo == null) return NotFound();

      if (photo.IsMain) return BadRequest("you can not delete your main photo");

      if (photo.PublicId != null)
      {
        var result = await _photoService.DeletePhotoAsync(photo.PublicId);
        if (result.Error != null) BadRequest(result.Error.Message);
      }

      user.Photos.Remove(photo);

      if (await _userRepository.SaveAllAsync()) return Ok();

      return BadRequest("Failed to delete the photo");


    }
  }
}