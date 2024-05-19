using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationSW.DataBase;
using PayStationSW.Devices;

namespace PayStationSW.RESTAPI
{
    [Route("api/PrinterDevice")]
    [ApiController]
    public class PrinterDeviceController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly DeviceService _deviceService;

        public PrinterDeviceController(ApplicationDbContext context, DeviceService deviceService)
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
                var printerDevice = station.Devices[DeviceEnum.Printer];
                return Ok(printerDevice.Config.IsEnabled);  // Ritorna direttamente il valore booleano
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("PrintTestPage")]
        public async Task<IActionResult> PrintTestPage()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var printer = station.Devices[DeviceEnum.Printer] as PrinterDevice;
                if (!printer.Config.IsConnected)
                {
                    return BadRequest(new { error = "The Printer device is not a connected device." });
                }
                string response = await printer.PrintTestPage();
                return Ok(new { status = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /*

        [HttpPost("EnableDevice")]
        public async Task<IActionResult> EnableDevice([FromBody] bool enableDevice)
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var printerDevice = station.Devices[DeviceEnum.Printer];
                if (!printerDevice.IsConnected)
                {
                    return BadRequest(new { error = "The printer device is not a connected device." });
                }
                string response = enableDevice ? await printerDevice.Enable() : await printerDevice.Disable();
                return Ok(new { status = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

















        [HttpPost("printTestPage")]
        public async Task<IActionResult> printTestPage()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.Devices[DeviceEnum.Printer].IsConnected)
                {
                    string response = await station.Devices[DeviceEnum.Printer].PrintTestPage();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The Printer device is not a connected." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        */
    }
}
