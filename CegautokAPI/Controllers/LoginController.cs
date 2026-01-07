using CegautokAPI.DTOs;
using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CegautokAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly Jwtsettings _jwtSettings;
        private readonly FlottaContext _context;

        public LoginController(Jwtsettings jwtSettings, FlottaContext context)
        {
            _jwtSettings = jwtSettings;
            _context = context;
        }

        [HttpGet]
        public IActionResult GetSalt(string loginName)
        {
            try
            {
                if (_context.Users.Select(u => u.LoginName).Contains(loginName))
                {
                    return Ok(_context.Users.FirstOrDefault(u => u.LoginName == loginName).Salt);
                }
                else
                {
                    return Ok("");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a kérés teljesítése közben: {ex.Message}");
            }

        }

        [HttpPost("Login")]
        public IActionResult Login(LoginDTO logindata)
        {
            try
            {
                string doubleHash = Program.CreateSHA256(logindata.SentHash);
                User user = _context.Users.FirstOrDefault(u => u.LoginName == logindata.LoginName && u.Hash == doubleHash && u.Active == true);
                if (user == null)
                {
                    return NotFound("Hibás bejelentkezési adatok.");
                }
                else
                {
                    var claims = new[]
                    {
                            new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _jwtSettings.Issuer,
                        audience: _jwtSettings.Audience,
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(_jwtSettings.ExpirityMinutes),
                        signingCredentials: creds
                        );
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a kérés teljesítése közben: {ex.Message}");
            }

        }
    }
}
