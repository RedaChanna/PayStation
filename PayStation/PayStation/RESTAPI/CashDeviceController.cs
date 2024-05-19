using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationSW.DataBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PayStationSW.Devices;



namespace PayStationSW.RESTAPI
{
    [Route("api/CashDevice")]
    [ApiController]
    public class CashDeviceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DeviceService _deviceService;

        public CashDeviceController(ApplicationDbContext context, DeviceService deviceService)
        {
            _context = context;
            _deviceService = deviceService;
        }



        [HttpPost("Reset")]
        public async Task<IActionResult> reset()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                var cashDevice = station.Devices[DeviceEnum.Cash] as CashDevice;
                if (!cashDevice.Config.IsConnected)
                {
                    return BadRequest(new { error = "The cash device is not a connected device." });
                }
                string response = await cashDevice.Reset();
                return Ok(new { status = response });


            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetEnableStatus")]
        public async Task<ActionResult<bool>> GetEnableStatus()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var cashDevice = station.Devices[DeviceEnum.Cash];
                return Ok(cashDevice.Config.IsEnabled);  // Ritorna direttamente il valore booleano
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetEnableStatusRC")]
        public async Task<ActionResult<bool>> GetEnableStatusRC()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var rcModule = station.Devices[DeviceEnum.RCModule];
                return Ok(rcModule.Config.IsEnabled);  // Ritorna direttamente il valore booleano
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("GetEnableStatusTwin")]
        public async Task<ActionResult<bool>> GetEnableStatusTwin()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var twinModule = station.Devices[DeviceEnum.TwinModule];
                return Ok(twinModule.Config.IsEnabled);  // Ritorna direttamente il valore booleano
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
                var cashDevice = station.Devices[DeviceEnum.Cash];
                if (!cashDevice.IsConnected)
                {
                    return BadRequest(new { error = "The cash device is not a connected device." });
                }
                string response = enableDevice ? await cashDevice.Enable() : await cashDevice.Disable();
                return Ok(new { status = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }



















        

        [HttpPost("reset")]
        public async Task<IActionResult> reset()
        { 
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.Devices[DeviceEnum.Cash].IsConnected)
                {
                    string response = "ok"; //await station.Devices[DeviceEnum.Cash].Reset();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The cash device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("enable")]
        public async Task<IActionResult> enable()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.Devices[DeviceEnum.Cash].IsConnected)
                {
                    string response = await station..Enable();
                    return Ok(new { status = response });
                }
                else
                {
                    string response = await station.CashDevice.Disable();
                    return Ok(new { status = response });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("disable")]
        public async Task<IActionResult> disable()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CashDevice.IsConnected)
                {
                    string response = await station.CashDevice.Disable();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The cash device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("payout")]
        public async Task<IActionResult> payout()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CashDevice.IsConnected)
                {
                    string response = await station.CashDevice.Payout();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The cash device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("powerUp")]
        public async Task<IActionResult> powerUp()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CashDevice.IsConnected)
                {
                    string response = await station.CashDevice.PowerUP();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The cash device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }*/
    }
}