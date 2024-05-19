namespace PayStationSW.RESTAPI
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] string message)
        {
            await WebSocketHandler.SendMessage(message);
            return Ok(new { message = "Message sent to all connected clients." });
        }
    }

}
