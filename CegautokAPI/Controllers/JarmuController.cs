using CegautokAPI.DTOs;
using CegautokAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAPI.Controllers
{
    [Route("/[controller]")]
    //[Authorize]
    [ApiController]
    
    public class JarmuController : ControllerBase
    {
        FlottaContext _context = new FlottaContext();
        public JarmuController(FlottaContext context)
        {
            _context = context;
        }
        //[Authorize]

        [Authorize]
        [HttpGet("Gepjarmus")]
        public IActionResult GetAllGepjarmus()
        {
            try
            {
                List<Gepjarmu> gepjarmus = _context.Gepjarmus.ToList();
                return Ok(gepjarmus);

            }
            catch (Exception ex)
            {
                return BadRequest(new Gepjarmu()
                {
                    Id = -1,
                    Rendszam = $"Hiba történt: {ex.Message}".Substring(0, 16),
                    Tipus = "hiba",
                    Ulesek = -1
                });
            }
        }

        [HttpGet("GepjarmuById/{Id}")]
        public IActionResult GetGepjarmuById(int Id)
        {
            try
            {
                var gepjarmu = _context.Gepjarmus.FirstOrDefault(u => u.Id == Id);
                if (gepjarmu is Gepjarmu)
                {
                    return Ok(gepjarmu);
                }
                else
                {
                    return BadRequest("Nincs ilyen gépjármű");

                }

            }
            catch (Exception ex)
            {
                return BadRequest(new Gepjarmu()
                {
                    Id = -1,
                    Rendszam = $"Hiba történt: {ex.Message}".Substring(0, 16),
                    Tipus = "hiba",
                    Ulesek = -1
                });
            }
        }


        [HttpPost("NewGepjarmu")]
        public IActionResult AddNewGepjarmu(Gepjarmu gepjarmu)
        {
            try
            {

                _context.Add(gepjarmu);
                _context.SaveChanges();
                return Ok("Sikeres rögzítés");

            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba történt a felvétel során: {ex.Message}");
            }
        }

        [HttpPut("ModifyGepjarmu")]
        public IActionResult ModifyGepjarmu(Gepjarmu gepjarmu)
        {
            try
            {
                if (_context.Gepjarmus.Contains(gepjarmu))
                {
                    _context.Update(gepjarmu);
                    _context.SaveChanges();
                    return Ok("Sikeres módosítás!");
                }
                else
                {
                    return BadRequest("Nincs ilyen gépjűrmű!");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a módosítás során: {ex.Message}");
            }
        }

        [HttpDelete("DelGepjarmu/{Id}")]
        public IActionResult DeleteGepjarmu(int Id)
        {
            try
            {
                if (_context.Gepjarmus.Select(u => u.Id).Contains(Id))
                {
                    Gepjarmu del = _context.Gepjarmus.FirstOrDefault(u => u.Id == Id);
                    _context.Remove(del);
                    _context.SaveChanges();
                    return Ok("Sikeres törlés!");
                }
                else
                {
                    return BadRequest("Nincs ilyen gépjármű!");
                }

            }
            catch (Exception ex)
            {
                return BadRequest($"Hiba a törlés közben: {ex.Message}");
            }
        }

        [HttpGet("{id}/Hasznalat")]
        
        public IActionResult GetHasznalatById(int id)
        {
                try
                {
                    
                    List<JarmuHasznalatDTO> valasz =_context.Kikuldottjarmus
                        .Include(k => k.Kikuldetes)
                        .Include(k => k.Gepjarmu)
                        .Where(j => j.GepjarmuId == id)
                        .Select(j => new JarmuHasznalatDTO()
                        {
                            Id = id,
                            Rendszam = j.Gepjarmu.Rendszam,
                            Kezdes = j.Kikuldetes.Kezdes,
                            Befejezes = j.Kikuldetes.Befejezes
                    })
                        .OrderBy(j => j.Kezdes)
                        .ToList();
                    return Ok(valasz);
                }
                catch (Exception)
                {
                    List<JarmuHasznalatDTO> valasz = new List<JarmuHasznalatDTO>() { new(){
                    Id = -1,
                    Rendszam = "hiba"} };
                    return BadRequest();
                }
            
        }

        [HttpGet("Sofor")]

        public IActionResult GetSofor()
        {
            
                try
                {

                    List<SoforDTO> valasz = _context.Kikuldottjarmus
                        .Include(k => k.Gepjarmu)
                        .Include(k => k.SoforNavigation)
                        .GroupBy(p => new { rsz = p.Gepjarmu.Rendszam, so = p.SoforNavigation.Name })
                        .Select(elem => new SoforDTO() {
                            Rendszam = elem.Key.rsz,
                            SoforNev = elem.Key.so,
                            Darab = elem.Count()
                            })
                        .ToList();
                    return Ok(valasz);
                }
                catch (Exception)
                {
                    List<JarmuHasznalatDTO> valasz = new List<JarmuHasznalatDTO>() { new(){
                    Id = -1,
                    Rendszam = "hiba"} };
                    return BadRequest();
                }
            
        }
    }
}
