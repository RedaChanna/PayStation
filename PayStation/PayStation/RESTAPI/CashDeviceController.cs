using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationSW.DataBase;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using PayStationSW.Devices;
using System.Security.Policy;



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


        #region Entity
        [HttpPost("EnableEntity")]
        public async Task<IActionResult> EnableEntity()
        {

            try
            {
                var _deviceType = "2";
                var _deviceDescription = "Vega Pro (lettore banconote)";
                var _deviceEnable = "1";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "1")
                {
                    status = "Cash device enable entity in data base updated successfully";
                }
                else
                {
                    status = "Cash device enable entity in data base went wrong";
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DisableEntity")]
        public async Task<IActionResult> DisableEntity()
        {

            try
            {
                var _deviceType = "2";
                var _deviceDescription = "Vega Pro (lettore banconote)";
                var _deviceEnable = "0";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "0")
                {
                    status = "Cash device disable entity in data base updated successfully";
                }
                else
                {
                    status = "Cash device disable entity in data base went wrong";
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetEntityStatus")]
        public async Task<ActionResult<DeviceEntityDto>> GetEntityStatus()
        {
            try
            {
                var deviceType = "2";
                var device = await _context.DevicesDB.FirstOrDefaultAsync(d => d.DeviceType == deviceType);

                if (device == null)
                {
                    return NotFound("Device not found, check the device type.");
                }

                var response = new DeviceEntityDto
                {
                    Status = $"Device {device.Description}: " + (device.Enabled == "1" ? "enabled." : "disabled."),
                    IsEnabled = device.Enabled == "1"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion
        #region Serial Connection
        [HttpGet("GetSerialConnectionParameter")]
        public async Task<ActionResult<SerialConnectionParameterDB>> GetSerialConnectionParameter()
        {
            try
            {
                var deviceName = DeviceEnum.Cash;
                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    return BadRequest("Invalid device name or device not founded.");
                }

                var device = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (device == null)
                {
                    return NotFound("Serial parameter for cash device not found in DB.");
                }

                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpPost("SerialConnectionParameter")]
        public async Task<ActionResult> SerialConnectionParameter([FromBody] SerialConnectionParameterDB deviceParameters)
        {
            try
            {
                deviceParameters.Device = DeviceEnum.Cash;
                if (deviceParameters == null)
                {
                    return BadRequest("Device parameters object is null.");
                }

                // Check if the device with the given name exists in the database
                var existingDevice = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceParameters.Device)
                    .FirstOrDefaultAsync();

                if (existingDevice == null)
                {
                    // If the device doesn't exist, create a new one
                    existingDevice = new SerialConnectionParameterDB
                    {
                        Device = deviceParameters.Device,
                        LastPortName = deviceParameters.LastPortName,
                        BaudRate = deviceParameters.BaudRate,
                        DataBits = deviceParameters.DataBits,
                        Parity = deviceParameters.Parity,
                        StopBits = deviceParameters.StopBits,
                        Handshake = deviceParameters.Handshake
                    };
                    // Add the new device to the context
                    _context.SerialConnectionParametersDB.Add(existingDevice);
                }
                else
                {
                    existingDevice.Device = deviceParameters.Device;
                    existingDevice.LastPortName = deviceParameters.LastPortName;
                    existingDevice.BaudRate = deviceParameters.BaudRate;
                    existingDevice.DataBits = deviceParameters.DataBits;
                    existingDevice.Parity = deviceParameters.Parity;
                    existingDevice.StopBits = deviceParameters.StopBits;
                    existingDevice.Handshake = deviceParameters.Handshake;
                }

                // Save changes to the database
                await _context.SaveChangesAsync();

                return Ok("Cash serial parameters are updated successfully in data base.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        
        [HttpDelete("DeleteSerialConnectionParameter")]
        public async Task<IActionResult> DeleteSerialConnectionParameter()
        {
            try
            {

                var deviceName = DeviceEnum.Cash;
                // Cerca il dispositivo con il nome dato nel database
                var existingDevice = await _context.SerialConnectionParametersDB
                    .FirstOrDefaultAsync(x => x.Device == deviceName);

                if (existingDevice == null)
                {
                    return NotFound("Cash's serial parameter not found in database.");
                }

                // Rimuove il dispositivo dal database
                _context.SerialConnectionParametersDB.Remove(existingDevice);

                // Salva le modifiche nel database
                await _context.SaveChangesAsync();

                return Ok("Cash serial parameter are deleted successfully from database.");
            }
            catch (Exception ex)
            {
                // Log dell'errore (aggiungi il logging appropriato qui)
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("SerialConnect")]
        public async Task<IActionResult> SerialConnect()
        {
            try
            {
                var deviceName = DeviceEnum.Cash;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var cash = station.Devices[deviceName] as CashDevice;
                if (cash == null)
                {
                    return NotFound("Cash device not found.");
                }
                if (!cash.Config.IsEnabled)
                {
                    return NotFound("Cash device is disable.");
                }
                var deviceConfig = cash.Config;
                var deviceSerialParameters = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (deviceSerialParameters == null)
                {
                    return NotFound("Cash device serial parameters not found in DB.");
                }
                deviceConfig.serialConnectionParameter = deviceSerialParameters;
                var connectResult = cash.Connect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (cash.Config.IsConnected)
                {
                    return Ok("Cash device is connected");
                }
                else
                {
                    return Ok("Cash device can not connecte");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("SerialDisconnect")]
        public async Task<IActionResult> SerialDisconnect()
        {
            try
            {
                var deviceName = DeviceEnum.Cash;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var cash = station.Devices[deviceName] as CashDevice;
                if (cash == null)
                {
                    return NotFound("Cash device not found.");
                }
                if (!cash.Config.IsEnabled)
                {
                    return NotFound("Cash device is disable.");
                }

                var connectResult = cash.Disconnect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (!cash.Config.IsConnected)
                {
                    return Ok("Cash device is disconnected");
                }
                else
                {
                    return Ok("Cash device can not be disconnected");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion
        #region Enable Disable Device
        [HttpPost("EnableDevice")]
        public async Task<IActionResult> EnableDevice()
        {
            try
            {
                var deviceName = DeviceEnum.Cash;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var cash = station.Devices[deviceName] as CashDevice;
                if (cash == null)
                {
                    return NotFound("Cash device not found.");
                }
                cash.Enable();
                return Ok("Cash Device flag IsEnable : enabled");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DisableDevice")]
        public async Task<IActionResult> DisableDevice()
        {

            try
            {
                var deviceName = DeviceEnum.Cash;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var cash = station.Devices[deviceName] as CashDevice;
                if (cash == null)
                {
                    return NotFound("Cash device not found.");
                }
                cash.Disable();
                return Ok("Cash Device flag IsEnable : disabled");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetEnStatusDevice")]
        public async Task<ActionResult<bool>> GetEnStatusDevice()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var cashDevice = station.Devices[DeviceEnum.Cash];
                var status = "";
                if (cashDevice.Config.IsEnabled)
                {
                    status = "Cash Device is enable";
                }
                else
                {
                    status = "Cash Device is disable";
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("EnableDeviceRC")]
        public async Task<IActionResult> EnableDeviceRC()
        {
            try
            {
                var deviceName = DeviceEnum.Cash;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var cash = station.Devices[deviceName] as CashDevice;
                if (cash == null)
                {
                    return NotFound("Cash device not found.");
                }
                cash.RCModule.Enable();
                cash.TwinModule.Disable();
                return Ok("RC in Cash Device flag IsEnable : enabled");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DisableDeviceRC")]
        public async Task<IActionResult> DisableDeviceRC()
        {

            try
            {
                var deviceName = DeviceEnum.Cash;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var cash = station.Devices[deviceName] as CashDevice;
                if (cash == null)
                {
                    return NotFound("Cash device not found.");
                }
                cash.RCModule.Disable();
                return Ok("RC in Cash Device flag IsEnable : disabled");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetEnStatusDeviceRC")]
        public async Task<ActionResult<bool>> GetEnStatusDeviceRC()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var cashDevice = station.Devices[DeviceEnum.Cash] as CashDevice;
                var status = "";
                if (cashDevice == null)
                {
                    return NotFound("Cash device not found.");
                }
                if (cashDevice.RCModule.Config.IsEnabled)
                {
                    status = "RC in Cash Device is enable";
                }
                else
                {
                    status = "RC in Cash Device is disable";
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("EnableDeviceTwin")]
        public async Task<IActionResult> EnableDeviceTwin()
        {
            try
            {
                var deviceName = DeviceEnum.Cash;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var cash = station.Devices[deviceName] as CashDevice;
                if (cash == null)
                {
                    return NotFound("Cash device not found.");
                }
                cash.TwinModule.Enable();
                cash.RCModule.Disable();
                return Ok("Twin in Cash Device flag IsEnable : enabled");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("DisableDeviceTwin")]
        public async Task<IActionResult> DisableDeviceTwin()
        {

            try
            {
                var deviceName = DeviceEnum.Cash;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var cash = station.Devices[deviceName] as CashDevice;
                if (cash == null)
                {
                    return NotFound("Cash device not found.");
                }
                cash.TwinModule.Disable();
                return Ok("Twin in Cash Device flag IsEnable : disabled");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("GetEnStatusDeviceTwin")]
        public async Task<ActionResult<bool>> GetEnStatusDeviceTwin()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var cashDevice = station.Devices[DeviceEnum.Cash] as CashDevice;
                var status = "";
                if (cashDevice == null)
                {
                    return NotFound("Cash device not found.");
                }
                if (cashDevice.TwinModule.Config.IsEnabled)
                {
                    status = "Twin in Cash Device is enable";
                }
                else
                {
                    status = "Twin in Cash Device is disable";
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion
        #region Presetting
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
        #endregion


        /*
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