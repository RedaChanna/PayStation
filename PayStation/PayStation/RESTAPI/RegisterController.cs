using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using PayStationSW.DataBase;
using PayStationSW.Devices;
using System.Diagnostics.Eventing.Reader;

namespace PayStationSW.RESTAPI
{
    [Route("api/Register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {

        private readonly StationManagerWS _stationManagerWS;
        private readonly ApplicationDbContext _context;
        private readonly DeviceService _deviceService;

        public RegisterController(ApplicationDbContext context, DeviceService deviceService, StationManagerWS stationManagerWS)
        {
            _context = context;
            _deviceService = deviceService;
            _stationManagerWS = stationManagerWS;

        }

        [HttpPost("EnableEntity")]
        public async Task<IActionResult> EnableEntity([FromBody] DeviceEntityDB device)
        {
            try
            {
                // Delega la gestione del database al DeviceService
                if ((device.DeviceType == "5") && (device.Enabled == "1"))
                {
                    var deviceResult1 = await _deviceService.ManageDeviceAsync("6", "", "0");
                }
                else if ((device.DeviceType == "6") && (device.Enabled == "1"))
                {
                    var deviceResult1 = await _deviceService.ManageDeviceAsync("5", "", "0");
                }
                var deviceResult = await _deviceService.ManageDeviceAsync(device.DeviceType ?? "", device.Description ?? "", device.Enabled ?? "0");  
                return Ok("Device parameters updated successfully: " + deviceResult.Description);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("AutomaticInit")]
        public async Task<IActionResult> AutomaticInit()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                string response = await station.ReconfigureDevices(_context);
                return Ok(new { status = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("SetImporto")]
        public async Task<IActionResult> SetImporto([FromBody] int importo)
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                if (!station.IsEnabled)
                {
                    return BadRequest(new { error = "The PayStation is not enable." });
                }
                Console.WriteLine($"Set Importo called {importo}");
                //Her I need to proccess the payment

                // Create a new MovementDB instance
                var movement = new MovementDB
                {
                    Amount = importo,
                    MovementDateOpen = DateTime.Now
                };

                // Process the payment
               // var paymentProcessor = new PaymentProcessor(movement);
                //paymentProcessor.StartPaymentProcess();

                // Add the movement to the database and save changes
                _context.MovementsDB.Add(movement);
                await _context.SaveChangesAsync();

                var cashDevice = station.Devices[DeviceEnum.Cash] as CashDevice;
                if (!cashDevice.Config.IsConnected)
                {
                    return BadRequest(new { error = "The cash device is not a connected device." });
                }
                else
                {
                    cashDevice.StartPolling();
                }


                _stationManagerWS.StartPeriodicMessages();
                return Ok(new { message = $"Set importo recived for importo {importo}, id movment is {movement.Id}." });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // GET: api/Register/GetAlarms
        [HttpGet("GetAlarms")]
        public async Task<IActionResult> GetAlarms()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                if (!station.InAlarm)
                {
                    return Ok("There is an allarm");
                }
                else
                {
                    return Ok("No allarm");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        // GET: api/Register/GetStatus
        [HttpGet("GetStatus")]
        public async Task<IActionResult> GetStaus()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
  
                    return Ok("1,100");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/DataBase/GetMovment/{id}
        [HttpGet("GetMovment/{id}")]
        public async Task<ActionResult<MovementDB>> GetMovementById(int? id)
        {
            if (id == null)
            {
                return BadRequest("Id cannot be null");
            }
            try
            {
                var movement = await _context.MovementsDB.FindAsync(id);
                if (movement == null)
                {
                    return NotFound($"Movement with id {id} not found");
                }
                return Ok(movement);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }


    public class ParametriPayment
    {
        public int Amount { get; set; }
        public DateTime DateSend { get; set; }

    }
}
