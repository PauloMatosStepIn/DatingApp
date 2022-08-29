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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
  public class AccountController : BaseApiController
  {
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService)
    {
      _tokenService = tokenService;
      _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {

      if (await UserExists(registerDto.Username)) return BadRequest("Username is Taken");

      using var hmac = new HMACSHA512();

      var user = new AppUser
      {
        UserName = registerDto.Username.ToLower(),
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
        PasswordSalt = hmac.Key
      };
      _context.Users.Add(user);
      await _context.SaveChangesAsync();

      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user)
      };
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {

      var user = await _context.Users.SingleOrDefaultAsync(u => u.UserName == loginDto.Username.ToLower());

      if (user == null) return Unauthorized("invalid user");

      using var hmac = new HMACSHA512(user.PasswordSalt);

      var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

      for (int i = 0; i < computeHash.Length; i++)
      {
        if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
      }

      return new UserDto
      {
        Username = user.UserName,
        Token = _tokenService.CreateToken(user)
      };
    }

    private async Task<bool> UserExists(string username)
    {

      return await _context.Users.AnyAsync(u => u.UserName == username.ToLower());
    }
  }
}