using CegautokAPI.DTOs;
using CegautokAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CegautokAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class KikuldetesController : ControllerBase
    {
        [HttpGet("Jarmuvek")]

        public IActionResult GetKikuldottJarmuvek()
        {
            using (var context = new FlottaContext())
            {
                try
                {
                    List<KikuldJarmu> valasz = context.Kikuldottjarmus
                        .Include(j => j.Kikuldetes)
                        .Include(j => j.Gepjarmu)
                        .Select(j => new KikuldJarmu()
                        {
                            Cim = j.Kikuldetes.Cim,
                            Datum = j.Kikuldetes.Kezdes,
                            Rendszam = j.Gepjarmu.Rendszam
                        })
                        .ToList();
                    return Ok(valasz);
                }
                catch (Exception ex)
                {
                    List<KikuldJarmu> valasz = new() { new KikuldJarmu(){
                    Cim = ex.Message} };
                    return BadRequest(valasz);
                }
            }
        }
    }
}
