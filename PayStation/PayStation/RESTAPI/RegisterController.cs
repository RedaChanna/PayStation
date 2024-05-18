using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationName.DataBase;
using PayStationName.Devices;

namespace PayStationName.RESTAPI
{
    [Route("api/Register")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly DeviceService _deviceService;

        public RegisterController(ApplicationDbContext context, DeviceService deviceService)
        {
            _context = context;
            _deviceService = deviceService;
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
                Console.WriteLine($"Set Importo called {importo}");
                //var station = await StationManager.GetStationAsync(_context);
                //string response = await station.ReconfigureDevices(_context);
                return Ok("ACK");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        /*

        [HttpPost("amountPayment")]
        public async Task<IActionResult> AmountPayment([FromBody] ParametriPayment parametri)
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);


                if (station.IsEnabled)
                {
                    Console.WriteLine(parametri.Amount);
                    Console.WriteLine(parametri.DateSend);

                    Console.WriteLine(DateTime.Now);
                    //Program.paymentStation.CoinMng.SetAmountToDeliver(parametri.Amount);
                    return Ok(new { status = "Request recived" });
                }
                else
                {
                    return BadRequest(new { error = "The request can't be handeled beacuse Paystation is offline." });
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }*/
    }


    public class ParametriPayment
    {
        public int Amount { get; set; }
        public DateTime DateSend { get; set; }

    }
}
