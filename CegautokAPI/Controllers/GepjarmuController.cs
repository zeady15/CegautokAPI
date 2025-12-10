using System.Reflection.Metadata.Ecma335;
using CegautokAPI.DTOs;
using CegautokAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GepjarmuController : ControllerBase
    {
        [Authorize]
        [HttpGet("Gepjarmu")]
        public IActionResult GetAll()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<Gepjarmu> gepjarmus = context.Gepjarmus.ToList();
                    return Ok(gepjarmus);
                }
                catch (Exception ex)
                {
                    List<Gepjarmu> valasz = new List<Gepjarmu>();
                    Gepjarmu hiba = new Gepjarmu()
                    {
                        Id = -1,
                        Rendszam = $"Hiba történt: {ex.Message}"
                    };
                    valasz.Add(hiba);
                    return BadRequest(valasz);
                }
            }
        }

        [HttpGet("GepjarmuById")]

        public IActionResult GetById(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                   Gepjarmu eredmeny = context.Gepjarmus.FirstOrDefault(g => g.Id == id);
                    if (eredmeny != null)
                    {
                        return Ok(eredmeny);
                    }
                    else
                    {
                        Gepjarmu hiba = new Gepjarmu()
                        {
                            Id = -1,
                            Rendszam = $"Hiba történt"
                        };
                        return NotFound(hiba);
                    }
                }
                catch (Exception ex)
                {
                    Gepjarmu hiba = new Gepjarmu()
                    {
                        Id = -1,
                        Rendszam = $"Hiba történt: {ex.Message}"
                    };
                    return BadRequest(hiba);
                }
            }
        }
        [HttpPost("NewGepjarmu")]
        public IActionResult NewGepjarmu(Gepjarmu gepjarmu)
        {
           using (var context = new FlottaContext())
            {
                try
                {
                    context.Gepjarmus.Add(gepjarmu);
                    context.SaveChanges();
                    return Ok("Sikeres rögzítés");
                }
                catch(Exception ex)
                { 
                    return BadRequest($"Hiba a rögzítés közben: {ex.Message}");
                }
            }

        }
        [HttpPut("ModifyGepjarmu")]
        public IActionResult ModifyGepjarmu(Gepjarmu gepjarmu)
        {
            using(var context = new FlottaContext())
            {
                try
                {
                    context.Gepjarmus.Update(gepjarmu);
                    context.SaveChanges();
                    return Ok("Sikeres módosítás");
                }
                catch( Exception ex)
                {
                    Gepjarmu hiba = new Gepjarmu()
                    {
                        Id = -1,
                        Rendszam = $"Hiba a módosítás közben: {ex.Message}"
                    };
                    return BadRequest(hiba);
                }
            }
        }
        [HttpDelete("DelGepjarmu")]
        public IActionResult DelGepjamu(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    if(context.Gepjarmus.Select(g => g.Id).Contains(id))
                    {
                        Gepjarmu del = new Gepjarmu{ Id = id };
                        context.Gepjarmus.Remove(del);
                    context.SaveChanges();
                    return Ok("Sikeres törlés");
                    }
                    else
                    {
                        return NotFound("Nincs ilyen Id");
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest($"Hiba a törlés közben: {ex.Message}");
                };
                
            }
        }
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
                catch(Exception)
                {
                    List<JarmuHasznalatDTO> valasz = new List<JarmuHasznalatDTO>()
                    {
                        new()
                        {
                            Id = -1,
                            Rendszam = "hiba" } };
                    return BadRequest(valasz);
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
                    .Include(j => j.Gepjarmu)
                    .Include(j => j.SoforNavigation)
                    .GroupBy(j => new {rsz = j.Gepjarmu.Rendszam, so = j.SoforNavigation.Name})
                    .Select(elem => new SoforDTO()
                    {
                        Rendszam = elem.Key.rsz,
                        Sofornev = elem.Key.so,
                        Darab = elem.Count()
                    })
                    .ToList();

                return Ok(valasz);
                }
                catch (Exception)
                {
                    List<SoforDTO> valasz = new List<SoforDTO>() { new()
                    {
                        Rendszam = "hiba" } };
                    return BadRequest();
                } 
            }
                
        }
        
    }
}
