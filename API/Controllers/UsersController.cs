using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using api.DTOs;
using api.Interfaces;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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

    // public UsersController(DataContext context)
    public UsersController(IUserRepository userRepository, IMapper mapper)
    {
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
    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
      // var user = await _context.Users.FindAsync(id);

      // var user = await _userRepository.GetUserByIdUsernameAsync(username);
      // var userToReturn = _mapper.Map<MemberDto>(user);
      // return Ok(userToReturn);

      var member = await _userRepository.GetMemberAsync(username);
      return Ok(member);
    }
  }
}