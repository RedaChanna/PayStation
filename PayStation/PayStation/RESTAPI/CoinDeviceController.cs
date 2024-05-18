using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationName.DataBase;
using PayStationName.Devices;

namespace PayStationName.RESTAPI
{
    [Route("api/CoinDevice")]
    [ApiController]
    public class CoinDeviceController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly DeviceService _deviceService;

        public CoinDeviceController(ApplicationDbContext context, DeviceService deviceService)
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
                var coinDevice = station.Devices[DeviceEnum.Coin];
                return Ok(coinDevice.Config.IsEnabled);  // Ritorna direttamente il valore booleano
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        
        [HttpPost("EnableDisableDevice")]
        public async Task<IActionResult> EnableDisableDevice([FromBody] bool enableDevice)
        {
            Console.WriteLine("EnableDisableDevice API called");
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (!coinDevice.Config.IsConnected)
                {
                    return BadRequest(new { error = "The Coin device is not a connected device." });
                }
                string response = enableDevice ? await coinDevice.EnableCommand() : await coinDevice.DisableCommand();
                return Ok(new { status = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }










        /*



       

        [HttpPost("reset")]
        public async Task<IActionResult> reset()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CoinDevice.IsConnected)
                {
                    string response = await station.CoinDevice.Reset();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The coin device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("setUpFeauture")]
        public async Task<IActionResult> setUpFeauture()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CoinDevice.IsConnected)
                {
                    string response = await station.CoinDevice.SetUpFeauture();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The coin device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("setUp")]
        public async Task<IActionResult> setUp()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CoinDevice.IsConnected)
                {
                    string response = await station.CoinDevice.SetUp();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The coin device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("expansionSetUp")]
        public async Task<IActionResult> expansionSetUp()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CoinDevice.IsConnected)
                {
                    string response = await station.CoinDevice.ExpansionSetUp();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The coin device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("enable")]
        public async Task<IActionResult> Enable()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CoinDevice.IsConnected)
                {
                    string response = await station.CoinDevice.Enable();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The coin device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("disable")]
        public async Task<IActionResult> Disable()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CoinDevice.IsConnected)
                {
                    string response = await station.CoinDevice.Disable();
                    return Ok(new { status = response });
                }
                else
                {
                    return BadRequest(new { error = "The coin device is not a connected device." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("deliverCoin")]
        public async Task<IActionResult> DeliverCoin([FromBody] int Amount)
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                if (station.CoinDevice.IsConnected)
                {
                    station.CoinDevice.SetAmountToDeliver(Amount);
                    return Ok(new { status = "Coin delivered" });
                }
                else
                {
                    return BadRequest(new { error = "The coin device is not a connected device." });
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
