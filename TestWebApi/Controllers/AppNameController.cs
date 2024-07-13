using Microsoft.AspNetCore.Mvc;

namespace TestWebApi.Controllers
{
    [ApiController]
    [Route("AppName")]
    public class AppNameController(IConfiguration configuration) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAppName()
        {
            return Ok(configuration["AppName"]); 
        }
    }
}
