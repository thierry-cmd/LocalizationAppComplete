using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace localizationApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {

        private readonly AppOptions _appOptions;

        public InfoController(IOptions<AppOptions> appOptions)
        {
            _appOptions = appOptions.Value;
        }

        [HttpGet]
        public IActionResult GetInfo()
        {
            var info = new
            {
                ServiceName = _appOptions.ServiceName,
                Environment = _appOptions.EnvironmentLabel,
                Timestamp = DateTime.UtcNow
            };

            return Ok(info);
        }
    }
}
