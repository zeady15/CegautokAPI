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
        //[Authorize]
        [HttpGet("{id}/Hasznalat")]
        
        public IActionResult GetHasznalatById(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    
                    List<JarmuHasznalatDTO> valasz = context.Kikuldottjarmus
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
        }

        [HttpGet("Sofor")]

        public IActionResult GetSofor()
        {
            using (var context = new FlottaContext())
            {
                try
                {

                    List<SoforDTO> valasz = context.Kikuldottjarmus
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
}
