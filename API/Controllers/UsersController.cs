using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Controllers;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
  //All the Users Controller needs authentication
  [Authorize]
  public class UsersController : BaseApiController
  {
    private readonly DataContext _context;
    public UsersController(DataContext context)
    {
      _context = context;
    }

    //[AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {

      var users = await _context.Users.ToListAsync();

      return users;
    }

    // api/users/3
    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {

      var user = await _context.Users.FindAsync(id);

      return user;
    }
  }
}