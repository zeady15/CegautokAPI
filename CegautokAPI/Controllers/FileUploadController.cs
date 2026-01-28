using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace CegautokAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        [HttpPost]
        [Route("ToFtpServer")]
        public async Task<IActionResult> FileUploadToFTP()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string fileName = postedFile.FileName;
                Stream fileStream = postedFile.OpenReadStream();
                string valasz = await Program.UploadToFtpServer(fileStream,fileName);//ide kerül a feltöltést végző függvény
                return Ok(valasz);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server errror"+ ex.Message);
            }
        }
    }
}
