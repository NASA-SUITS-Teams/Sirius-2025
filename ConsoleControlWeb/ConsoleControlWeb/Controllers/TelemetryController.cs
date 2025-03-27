using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleControlWeb.Controllers
{
    [ApiController]
    [Route("api/telemetry")]
    public class TelemetryController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        // Updated URL: note the inclusion of "json_data" based on your folder structure.
        private readonly string telemetryUrl = "http://127.0.0.1:14141/json_data/teams/0/ROVER_TELEMETRY.json";

        public TelemetryController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetTelemetry()
        {
            try
            {
                var response = await _httpClient.GetAsync(telemetryUrl);
                response.EnsureSuccessStatusCode();
                var jsonString = await response.Content.ReadAsStringAsync();
                return Content(jsonString, "application/json");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
