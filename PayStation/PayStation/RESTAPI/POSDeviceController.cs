using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationSW.DataBase;
using PayStationSW.Devices;

namespace PayStationSW.RESTAPI
{
    [Route("api/POSDevice")]
    [ApiController]
    public class POSDeviceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DeviceService _deviceService;

        public POSDeviceController(ApplicationDbContext context, DeviceService deviceService)
        {
            _context = context;
            _deviceService = deviceService;
        }



        [HttpGet("GetEnableStatus")]
        public async Task<ActionResult<bool>> GetEnableStatus()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var posDevice = station.Devices[DeviceEnum.Pos];
                return Ok(posDevice.Config.IsEnabled);  // Ritorna direttamente il valore booleano
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }





        /*
        [HttpPost("EnableDevice")]
        public async Task<IActionResult> EnableDevice([FromBody] bool enableDevice)
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var posDevice = station.Devices[DeviceEnum.POS];
                if (!posDevice.IsConnected)
                {
                    return BadRequest(new { error = "The POS device is not a connected device." });
                }
                string response = enableDevice ? await posDevice.Enable() : await posDevice.Disable();
                return Ok(new { status = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        */
    }
}
