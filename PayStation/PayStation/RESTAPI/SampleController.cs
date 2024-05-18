using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class SampleController : ControllerBase
{
    private readonly StationManagerWS _stationManagerWS;

    public SampleController(StationManagerWS stationManagerWS)
    {
        _stationManagerWS = stationManagerWS;
    }

    [HttpPost("trigger-event")]
    public IActionResult TriggerEvent()
    {
        _stationManagerWS.StartPeriodicMessages();
        return Ok(new { message = "Timer started and clients will be notified every 10 seconds." });
    }
}
