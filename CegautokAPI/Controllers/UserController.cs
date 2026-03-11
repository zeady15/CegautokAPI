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
        
        public UserController(FlottaContext context)
        {
            _context = context;
        }

        [HttpGet("Users")]
        public IActionResult GetUsers()
        {
                try
                {
                    List<User> users = _context.Users.ToList();
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

        [HttpGet("UserById")]
        public IActionResult GetUserById(int id)
        {
           try
                {
                    User eredmeny = _context.Users.Include(u => u.PermissionNavigation).FirstOrDefault(x=> x.Id == id);
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

        [HttpPost("NewUser")]
        public IActionResult PostUser(User user)
        {
                try
                {
                    _context.Users.Add(user);
                    _context.SaveChanges();
                    return Ok("Sikeres rögzítés");
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a rögzítés közben {ex.Message}");
                }
            
        }

        [HttpPut("ModifyUser")]
        public IActionResult PutUser(User user)
        {
           
            
                try
                {
                    if (_context.Users.Contains(user))
                    {
                        _context.Users.Update(user);
                        _context.SaveChanges();
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

        [HttpDelete("DelUser")]
        public IActionResult DeleteUser(int id)
        {
            
            
                try
                {
                    if (_context.Users.Select(u => u.Id).Contains(id))
                    {
                        _context.Remove(new  User { Id = id });
                        _context.SaveChanges();
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
