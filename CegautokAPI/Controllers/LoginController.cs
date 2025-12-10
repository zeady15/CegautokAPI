using CegautokAPI.DTOs;
using CegautokAPI.Models;
using CegautokAPI.Models;
using CegautokAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;


namespace CegautokAP.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        public LoginController(JwtSettings jwtSettings)
        {
            _jwtSettings = jwtSettings;
        }
        [HttpGet("GetSalt")]
        public IActionResult GetSalt(string username)
        {
            try
            {
                using (var context = new FlottaContext())
                {
                    if (context.Users.Any(u => u.LoginName == username))
                    {
                        return Ok(context.Users.First(u => u.LoginName == username).Salt);
                    }
                    else
                    {
                        return BadRequest("Nincs ilyen felhasználónév!");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest("hiba: " + ex.Message);
            }

        }

        [HttpPost("Login")]
        public IActionResult Login(LoginDTO loginDTO)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    string doubleHash = Program.CreateSHA256(loginDTO.Hash);
                    User user = context.Users.FirstOrDefault(u => u.LoginName == loginDTO.LoginName && u.Hash == doubleHash && u.Active);
                    if (user == null)
                    {
                        return NotFound("Nincs megfelelő felhasználó! A belépés sikertelen!");
                    }
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.LoginName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                    var creds = new SigningCredentials(key,SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(issuer: _jwtSettings.Issuer,
                        audience: _jwtSettings.Audience,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirityMinutes),
                        signingCredentials: creds);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));

                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a bejelentkezés során: {ex.Message}");
                }
            }
        }
    }
}