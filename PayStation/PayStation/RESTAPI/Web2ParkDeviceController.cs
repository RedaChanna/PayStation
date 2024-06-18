using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationSW.DataBase;
using PayStationSW.Devices;
using Microsoft.EntityFrameworkCore;

namespace PayStationSW.RESTAPI
{
    [Route("api/Web2ParkDevice")]
    [ApiController]
    public class Web2ParkDeviceController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly DeviceService _deviceService;

        public Web2ParkDeviceController(ApplicationDbContext context, DeviceService deviceService)
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
                var _deviceType = "8";
                var _deviceDescription = "Web 2 Park";
                var _deviceEnable = "1";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "1")
                {
                    status = "Web 2 Park enable entity in data base updated successfully";
                }
                else
                {
                    status = "Web 2 Park enable entity in data base went wrong";
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
                var _deviceType = "8";
                var _deviceDescription = "Web 2 Park";
                var _deviceEnable = "0";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "0")
                {
                    status = "Web 2 Park disable entity in data base updated successfully";
                }
                else
                {
                    status = "Web 2 Park disable entity in data base went wrong";
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
                var deviceType = "8";
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
                var deviceName = DeviceEnum.Web2Park;
                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    return BadRequest("Invalid device name or device not founded.");
                }

                var device = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (device == null)
                {
                    return NotFound("Serial parameter for Web2Park not found in DB.");
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
                deviceParameters.Device = DeviceEnum.Web2Park;
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

                return Ok("Web2Park serial parameters are updated successfully in data base.");
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

                var deviceName = DeviceEnum.Web2Park;
                // Cerca il dispositivo con il nome dato nel database
                var existingDevice = await _context.SerialConnectionParametersDB
                    .FirstOrDefaultAsync(x => x.Device == deviceName);

                if (existingDevice == null)
                {
                    return NotFound("Web2Park's serial parameter not found in database.");
                }

                // Rimuove il dispositivo dal database
                _context.SerialConnectionParametersDB.Remove(existingDevice);

                // Salva le modifiche nel database
                await _context.SaveChangesAsync();

                return Ok("Web2Park serial parameter are deleted successfully from database.");
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
                var deviceName = DeviceEnum.Web2Park;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var web2Park = station.Devices[deviceName] as Web2ParkDevice;
                if (web2Park == null)
                {
                    return NotFound("Web2Park device not found.");
                }
                if (!web2Park.Config.IsEnabled)
                {
                    return NotFound("Web2Park device is disable.");
                }
                var deviceConfig = web2Park.Config;
                var deviceSerialParameters = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (deviceSerialParameters == null)
                {
                    return NotFound("Web2Park device serial parameters not found in DB.");
                }
                deviceConfig.serialConnectionParameter = deviceSerialParameters;
                var connectResult = web2Park.Connect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (web2Park.Config.IsConnected)
                {
                    return Ok("Web2Park device is connected");
                }
                else
                {
                    return Ok("Web2Park device can not connecte");
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
                var deviceName = DeviceEnum.Web2Park;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var web2Park = station.Devices[deviceName] as Web2ParkDevice;
                if (web2Park == null)
                {
                    return NotFound("Web2Park device not found.");
                }
                if (!web2Park.Config.IsEnabled)
                {
                    return NotFound("Web2Park device is disable.");
                }

                var connectResult = web2Park.Disconnect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (!web2Park.Config.IsConnected)
                {
                    return Ok("Web2Park device is disconnected");
                }
                else
                {
                    return Ok("Web2Park device can not be disconnected");
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
                var deviceName = DeviceEnum.Web2Park;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var web2Park = station.Devices[deviceName] as Web2ParkDevice;
                if (web2Park == null)
                {
                    return NotFound("Web2Park device not found.");
                }
                web2Park.Enable();
                return Ok("Web2Park Device flag IsEnable : enabled");
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
                var deviceName = DeviceEnum.Web2Park;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var web2Park = station.Devices[deviceName] as Web2ParkDevice;
                if (web2Park == null)
                {
                    return NotFound("Web2Park device not found.");
                }
                web2Park.Disable();
                return Ok("Web2Park Device flag IsEnable : disabled");
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
                var web2Park = station.Devices[DeviceEnum.Web2Park];
                var status = "";
                if (web2Park.Config.IsEnabled)
                {
                    status = "Web2Park Device is enable";
                }
                else
                {
                    status = "Web2Park Device is disable";
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion










        [HttpPost("StartPolling")]
        public async Task<IActionResult> StartPolling()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var web2ParkDevice = station.Devices[DeviceEnum.Web2Park] as Web2ParkDevice;
                if (web2ParkDevice == null)
                {
                    return BadRequest(new { error = "The Web2Park is not present as device in PayStation." });
                }
                if (!web2ParkDevice.Config.IsConnected)
                {
                    return BadRequest(new { error = "The Web2Park serial port is not open." });
                }
                if (!web2ParkDevice.Config.IsEnabled)
                {
                    return BadRequest(new { error = "The Web2Park is not a enable as device in PayStation." });

                }

                web2ParkDevice.StartPolling();
                return Ok("Polling start correctly.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("StopPolling")]
        public async Task<IActionResult> StopPolling()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var web2ParkDevice = station.Devices[DeviceEnum.Web2Park] as Web2ParkDevice;
                if (web2ParkDevice == null)
                {
                    return BadRequest(new { error = "The Web2Park is not present as device in PayStation." });
                }
                web2ParkDevice.StopPolling();
                return Ok("Polling stoped correctly.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }








        [HttpPost("SendTestMessage")]
        public async Task<IActionResult> SendTestMessage()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var web2Park = station.Devices[DeviceEnum.Web2Park] as Web2ParkDevice;
                if (!web2Park.Config.IsConnected)
                {
                    return BadRequest(new { error = "The Web 2 Park device is not a connected device." });
                }
                string response = await web2Park.SendTestMessage();
                return Ok(new { status = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }




    }
}
