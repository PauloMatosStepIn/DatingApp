using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
//using Newtonsoft.Json;
using System.Text.Json;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data
{
  public class Seed
  {
    public static async Task SeedUsers(DataContext context)
    {
      // if Table have any user exit
      if (await context.Users.AnyAsync()) return;

      var userData = await System.IO.File.ReadAllTextAsync("Data/UserSeedData.json");

      //var users = JsonConvert.DeserializeObject<List<AppUser>>(userData);
      var users = JsonSerializer.Deserialize<List<AppUser>>(userData);


      if (users != null)
      {
        Console.WriteLine("users not null");
      }

      foreach (var user in users)
      {
        Console.WriteLine(user.UserName);
        using var hmac = new HMACSHA512();
        user.UserName = user.UserName.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
        user.PasswordSalt = hmac.Key;

        context.Users.Add(user);
      }

      await context.SaveChangesAsync();

    }
  }
}