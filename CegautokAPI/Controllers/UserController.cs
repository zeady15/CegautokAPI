using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Users")]
        public IActionResult GetAll()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<User> users = context.Users.ToList();
                    return Ok(users);
                }
                catch (Exception ex)
                {
                    List<User> valasz = new List<User>();
                    User hiba = new User()
                    {
                        Id = -1,
                        Name = $"Hiba történt: {ex.Message}",
                    };
                    valasz.Add(hiba);
                    return BadRequest(valasz);
                }
            }
        }

        [HttpGet("UserById")]
        public IActionResult GetById(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                   User eredmeny = context.Users.FirstOrDefault(u => u.Id == id);
                    if (eredmeny != null)
                    {
                        return Ok(eredmeny);
                    }
                    else
                    {
                        User hiba = new User()
                        {
                            Id = -1,
                            Name = $"Hiba történt",
                        };
                        return NotFound(hiba);
                    }
                }
                catch (Exception ex)
                {
                    User hiba = new User()
                    {
                        Id = -1,
                        Name = $"Hiba történt: {ex.Message}",
                    };
                    return BadRequest(hiba);
                }
            }
        }
    }
}
