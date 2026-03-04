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
