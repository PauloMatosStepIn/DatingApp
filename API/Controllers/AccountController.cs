using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using api.DTOs;
using api.Interfaces;
using API.Data;
using API.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  public class AccountController : BaseApiController
  {
    // private readonly DataContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;




    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    // public AccountController(DataContext context, ITokenService tokenService, IMapper mapper)

    //Change DataContext by UserManager & SignInManager
    public AccountController(
      UserManager<AppUser> userManager,
      SignInManager<AppUser> signInManager,
      ITokenService tokenService, IMapper mapper)
    {
      _mapper = mapper;
      _tokenService = tokenService;
      // _context = context;
      _userManager = userManager;
      _signInManager = signInManager;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

      if (await UserExists(registerDto.Username)) return BadRequest("Username is Taken");

      var user = _mapper.Map<AppUser>(registerDto);

      // done by AspNetIdentity
      // using var hmac = new HMACSHA512();

      user.UserName = registerDto.Username.ToLower();

      // done by AspNetIdentity
      // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
      // user.PasswordSalt = hmac.Key;

      // Chaged by AspNetUsers
      // _context.Users.Add(user);
      // await _context.SaveChangesAsync();

      var result = await _userManager.CreateAsync(user, registerDto.Password);

      if (!result.Succeeded) return BadRequest(result.Errors);

      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        KnownAs = user.KnownAs,
        Gender = user.Gender
      };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

      var user = await _userManager.Users
      .Include(p => p.Photos)
      .SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

      if (user == null) return Unauthorized("invalid user/password");

      var result = await _signInManager
      .CheckPasswordSignInAsync(user, loginDto.Password,false);

      if(!result.Succeeded) return Unauthorized("invalid user/password");

      // done by AspNetCore
      // using var hmac = new HMACSHA512(user.PasswordSalt);
      // var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

      // for (int i = 0; i < computeHash.Length; i++)
      // {
      //   if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
      // }

      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user),
        PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
        KnownAs = user.KnownAs,
        Gender = user.Gender
      };
    }

    private async Task<bool> UserExists(string username)
    {

      // return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
      return await _userManager.Users.AnyAsync(u => u.UserName == username.ToLower());
    }
  }
}