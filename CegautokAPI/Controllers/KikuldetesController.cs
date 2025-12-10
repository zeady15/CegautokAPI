using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CegautokAPI.DTOs;

namespace CegautokAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class KikuldetesController : ControllerBase
    {
        [HttpGet("Jarmuek")]
        public IActionResult GetAll()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<KikuldJarmu> valasz = context.Kikuldottjarmus.Include(x => x.Kikuldetes)
                        .Include(x => x.Gepjarmu)
                        .Select(x => new KikuldJarmu() 
                        {
                            Cim = x.Kikuldetes.Cim, 
                         Datum = x.Kikuldetes.Kezdes, 
                         Rendszam = x.Gepjarmu.Rendszam})
                        .ToList();
                    return Ok(valasz);
                }
                catch (Exception ex)
                {
                    List<Kikuldte> valasz = new List<Kikuldte>();
                    Kikuldte hiba = new Kikuldte()
                    {
                        Id = -1,
                        Celja = $"Hiba történt: {ex.Message}"
                    };
                    valasz.Add(hiba);
                    return BadRequest(valasz);
                }
            }
        }
        [HttpGet("KikuldtesById")]
        public IActionResult GetById(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    Kikuldte eredmeny = context.Kikuldtes.FirstOrDefault(k => k.Id == id);
                    if (eredmeny != null)
                    {
                        return Ok(eredmeny);
                    }
                    else
                    {
                        Kikuldte hiba = new Kikuldte()
                        {
                            Id = -1,
                            Celja = $"Hiba történt"
                        };
                        return NotFound(hiba);
                    }
                }
                catch (Exception ex)
                {
                    Kikuldte hiba = new Kikuldte()
                    {
                        Id = -1,
                        Celja = $"Hiba történt: {ex.Message}"
                    };
                    return BadRequest(hiba);
                }
            }
        }       
        [HttpGet("SoforByKikuldetesId/{id}")]
        public IActionResult GetSoforByKikuldId(int id)
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    var Sofor = context.Kikuldottjarmus
                        .Include(j => j.Kikuldetes)
                        .Include(j => j.SoforNavigation)
                        .FirstOrDefault(k => k.Id == id);
                    if(Sofor != null)
                    {
                        string SoforNeve = Sofor.SoforNavigation.Name;
                        return Ok(SoforNeve);
                    }
                    else
                    {
                        return NotFound("Nincs ilyen név");
                    }

                }
                catch(Exception ex)
                {
                    return BadRequest($"Hiba: {ex.Message}");
                }
            }
        }
    }
}
