using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class RegistryController : ControllerBase
    {
        private readonly FlottaContext _context;
        public RegistryController(FlottaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostReg(User user)
        {
            try
            {
                if (_context.Users.FirstOrDefault(u => u.LoginName == user.LoginName) !=null)
                {
                    return BadRequest("Foglalt felhasználónév.");
                } 
                if (_context.Users.FirstOrDefault(u => u.Email == user.Email) !=null)
                {
                    return BadRequest("Ez az email cím már használatban van.");
                }
                user.Active = false;
                user.Permission = 1;
                user.Hash = Program.CreateSHA256(user.Hash);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                Program.SendEmail(user.Email, "Regisztráció megerősítése", $"http://localhost:5171/Registry?felhasznaloNev={user.LoginName}&email={user.Email}");
                return Ok("Sikeres regisztráció, erősítse meg a megadott emailre kiküldött linkre kattintva.");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        public async Task<IActionResult> ConfirmReg(string felhasznaloNev, string email)
        {
            try
            {
                User? user = await _context.Users.FirstOrDefaultAsync(u => u.LoginName == felhasznaloNev && u.Email == email);
                if (user != null)
                {
                    user.Active = true;
                    user.Permission = 2;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                    return Ok("Sikeres megerősítés.");
                }
                else
                {
                    return BadRequest("Hibás adatok!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
