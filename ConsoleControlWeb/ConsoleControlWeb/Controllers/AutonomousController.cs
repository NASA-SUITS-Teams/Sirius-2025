using Microsoft.AspNetCore.Mvc;
using ConsoleControlWeb.Models;
using ConsoleControlWeb.Services;

namespace ConsoleControlWeb.Controllers
{
    [ApiController]
    [Route("api/autonomous")]
    public class AutonomousController : ControllerBase
    {
        private readonly AutonomousNavigationService _autoService;

        public AutonomousController(AutonomousNavigationService autoService)
        {
            _autoService = autoService;
        }

        public class AutonomousRequest
        {
            public float DestinationX { get; set; }
            public float DestinationY { get; set; }
        }

        [HttpPost("start")]
        public IActionResult StartAutonomous([FromBody] AutonomousRequest request)
        {
            _autoService.StartAutonomous(new Vector2(request.DestinationX, request.DestinationY));
            return Ok(new { status = "Autonomous navigation started" });
        }

        [HttpPost("stop")]
        public IActionResult StopAutonomous()
        {
            _autoService.StopAutonomous();
            return Ok(new { status = "Autonomous navigation stopped" });
        }
    }
}
