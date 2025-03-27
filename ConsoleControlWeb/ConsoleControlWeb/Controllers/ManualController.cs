using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ConsoleControlWeb.Controllers
{
    [ApiController]
    [Route("api/manual")]
    public class ManualController : ControllerBase
    {
        private readonly string serverIP = "127.0.0.1";
        private readonly int serverPort = 14141;

        public class ManualCommand
        {
            public float Throttle { get; set; }
            public float Steering { get; set; }
            public float Brakes { get; set; }
        }

        [HttpPost("command")]
        public async Task<IActionResult> SendManualCommand([FromBody] ManualCommand command)
        {
            // Log the received command
            Console.WriteLine($"[ManualController] Received command: Throttle={command.Throttle}, Steering={command.Steering}, Brakes={command.Brakes}");
            using (var client = new TelemetryClient(serverIP, serverPort))
            {
                // Send commands as per TSS spec:
                // 1107 = Brakes, 1109 = Throttle, 1110 = Steering.
                await client.SendCommandAsync(1107, command.Brakes);
                await client.SendCommandAsync(1109, command.Throttle);
                await client.SendCommandAsync(1110, command.Steering);
            }
            return Ok(new { status = "Manual command sent" });
        }
    }
}
