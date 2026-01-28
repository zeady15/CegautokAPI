using CegautokAPI.DTOs;
using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly FlottaContext _context;

        [HttpGet("Users")]
        public IActionResult GetUsers()
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
                    List<User> valasz = new()
                    {
                        new User { Id = -1,
                                   Name = "Hiba történt: "+ex.Message,
                        }
                    };
                    return BadRequest(valasz);
                }
            }
        }

        [HttpGet("UserById")]
        public IActionResult GetUserById(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    User eredmeny = context.Users.Include(u => u.PermissionNavigation).FirstOrDefault(x=> x.Id == id);
                    if (eredmeny != null)
                        return Ok(eredmeny);
                    else
                    {
                        User valasz = new User
                        {
                            Id = -1,
                            Name = "Hiba történt: nincs ilyen azonosítójú felhasználó!",
                        };
                        return NotFound(valasz);
                    }
                }
                catch (Exception ex)
                {
                    User valasz = new User { Id = -1,
                                             Name = "Hiba történt: "+ex.Message,
                                  };
                    return BadRequest(valasz);
                }
            }
        }

        [HttpPost("NewUser")]
        public IActionResult PostUser(User user)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                    return Ok("Sikeres rögzítés");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a rögzítés közben {ex.Message}");
                }
            }
        }

        [HttpPut("ModifyUser")]
        public IActionResult PutUser(User user)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    if (context.Users.Contains(user))
                    {
                        context.Users.Update(user);
                        context.SaveChanges();
                        return Ok("Sikeres rögzítés");
                    }
                    else
                    {
                        return NotFound("Nincs ilyen felhasználó!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a módosítás közben {ex.Message}");
                }
            }
        }

        [HttpDelete("DelUser")]
        public IActionResult DeleteUser(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    if (context.Users.Select(u => u.Id).Contains(id))
                    {
                        context.Remove(new  User { Id = id });
                        context.SaveChanges();
                        return Ok("Sikeres törlés");
                    }
                    else
                    {
                        return NotFound("Nincs ilyen felhasználó!");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a törlés közben {ex.Message}");
                }
            }
        }

        [HttpGet("Jarmuvek/{id}")]
        public IActionResult GetUserJarmuvek(int id) 
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<UserJarmuvekDTO> valasz = context.Kikuldottjarmus
                        .Include(k => k.Kikuldetes)
                        .Include(k => k.Gepjarmu)
                        .Include(k => k.SoforNavigation)
                        .Where(k => k.SoforNavigation.Id == id)
                        .Select(k => new UserJarmuvekDTO()
                        {
                            Id = id,
                            Name = k.SoforNavigation.Name,
                            Kezdes = k.Kikuldetes.Kezdes,
                            Rendszam = k.Gepjarmu.Rendszam
                        })
                        .ToList();
                    return Ok(valasz);
                }
                catch (Exception ex)
                {
                    List<UserJarmuvekDTO> valasz = new List<UserJarmuvekDTO>() { new UserJarmuvekDTO() { Id = id, Name = ex.Message } };
                    return BadRequest(valasz);
                }
            }
        }
    }
}
