using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SecureChatServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly ILogger _logger;
        public StatusController(ILogger<StatusController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public string Get()
        {
            _logger.LogInformation("Server status checked");
            return "Server is Up and Running!";
        }
    }
}
