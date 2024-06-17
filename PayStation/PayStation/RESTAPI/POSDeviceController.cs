using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationSW.DataBase;
using PayStationSW.Devices;
using Microsoft.EntityFrameworkCore;

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

        #region Entity
        [HttpPost("EnableEntity")]
        public async Task<IActionResult> EnableEntity()
        {

            try
            {
                var _deviceType = "3";
                var _deviceDescription = "Iself2000 (lettore POS)";
                var _deviceEnable = "1";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "1")
                {
                    status = "POS device enable entity in data base updated successfully";
                }
                else
                {
                    status = "POS device enable entity in data base went wrong";
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
                var _deviceType = "3";
                var _deviceDescription = "Iself2000 (lettore POS)";
                var _deviceEnable = "0";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "0")
                {
                    status = "POS device disable entity in data base updated successfully";
                }
                else
                {
                    status = "POS device disable entity in data base went wrong";
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
                var deviceType = "3";
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
                var deviceName = DeviceEnum.Pos;
                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    return BadRequest("Invalid device name or device not founded.");
                }

                var device = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (device == null)
                {
                    return NotFound("Serial parameter for pos device not found in DB.");
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
                deviceParameters.Device = DeviceEnum.Pos;
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

                return Ok("POS serial parameters are updated successfully in data base.");
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

                var deviceName = DeviceEnum.Pos;
                // Cerca il dispositivo con il nome dato nel database
                var existingDevice = await _context.SerialConnectionParametersDB
                    .FirstOrDefaultAsync(x => x.Device == deviceName);

                if (existingDevice == null)
                {
                    return NotFound("POS's serial parameter not found in database.");
                }

                // Rimuove il dispositivo dal database
                _context.SerialConnectionParametersDB.Remove(existingDevice);

                // Salva le modifiche nel database
                await _context.SaveChangesAsync();

                return Ok("POS serial parameter are deleted successfully from database.");
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
                var deviceName = DeviceEnum.Pos;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var pos = station.Devices[deviceName] as POSDevice;
                if (pos == null)
                {
                    return NotFound("POS device not found.");
                }
                if (!pos.Config.IsEnabled)
                {
                    return NotFound("POS device is disable.");
                }
                var deviceConfig = pos.Config;
                var deviceSerialParameters = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (deviceSerialParameters == null)
                {
                    return NotFound("POS device serial parameters not found in DB.");
                }
                deviceConfig.serialConnectionParameter = deviceSerialParameters;
                var connectResult = pos.Connect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (pos.Config.IsConnected)
                {
                    return Ok("POS device is connected");
                }
                else
                {
                    return Ok("POS device can not connecte");
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
                var deviceName = DeviceEnum.Pos;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var pos = station.Devices[deviceName] as POSDevice;
                if (pos == null)
                {
                    return NotFound("POS device not found.");
                }
                if (!pos.Config.IsEnabled)
                {
                    return NotFound("POS device is disable.");
                }

                var connectResult = pos.Disconnect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (!pos.Config.IsConnected)
                {
                    return Ok("POS device is disconnected");
                }
                else
                {
                    return Ok("POS device can not be disconnected");
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
                var deviceName = DeviceEnum.Pos;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var pos = station.Devices[deviceName] as POSDevice;
                if (pos == null)
                {
                    return NotFound("POS device not found.");
                }
                pos.Enable();
                return Ok("POS Device flag IsEnable : enabled");
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
                var deviceName = DeviceEnum.Pos;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var pos = station.Devices[deviceName] as POSDevice;
                if (pos == null)
                {
                    return NotFound("POS device not found.");
                }
                pos.Disable();
                return Ok("POS Device flag IsEnable : disabled");
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
                var posDevice = station.Devices[DeviceEnum.Pos];
                var status = "";
                if (posDevice.Config.IsEnabled)
                {
                    status = "POS Device is enable";
                }
                else
                {
                    status = "POS Device is disable";
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion


        [HttpPost("ActivatePos")]
        public async Task<IActionResult> ActivatePos()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var pos = station.Devices[DeviceEnum.Pos] as POSDevice;
                if (!pos.Config.IsConnected)
                {
                    return BadRequest(new { error = "The Pos device is not a connected device." });
                }
                string response = await pos.ActivatePOS();
                return Ok(new { status = response });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
