using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationSW.DataBase;
using PayStationSW.Devices;
using Microsoft.EntityFrameworkCore;

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

        #region Entity

        [HttpPost("EnableEntity")]
        public async Task<IActionResult> EnableEntity()
        {

            try
            {
                var _deviceType = "4";
                var _deviceDescription = "KP300 (stampante ticket)";
                var _deviceEnable = "1";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "1")
                {
                    status = "Printer device enable entity in data base updated successfully";
                }
                else
                {
                    status = "Printer device enable entity in data base went wrong";
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
                var _deviceType = "4";
                var _deviceDescription = "KP300 (stampante ticket)";
                var _deviceEnable = "0";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "0")
                {
                    status = "Printer device disable entity in data base updated successfully";
                }
                else
                {
                    status = "Printer device disable entity in data base went wrong";
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
                var deviceType = "4";
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
                var deviceName = DeviceEnum.Printer;
                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    return BadRequest("Invalid device name or device not founded.");
                }

                var device = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (device == null)
                {
                    return NotFound("Serial parameter for printer device not found in DB.");
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
                deviceParameters.Device = DeviceEnum.Printer;
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

                return Ok("Printer serial parameters are updated successfully in data base.");
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

                var deviceName = DeviceEnum.Printer;
                // Cerca il dispositivo con il nome dato nel database
                var existingDevice = await _context.SerialConnectionParametersDB
                    .FirstOrDefaultAsync(x => x.Device == deviceName);

                if (existingDevice == null)
                {
                    return NotFound("Printer's serial parameter not found in database.");
                }

                // Rimuove il dispositivo dal database
                _context.SerialConnectionParametersDB.Remove(existingDevice);

                // Salva le modifiche nel database
                await _context.SaveChangesAsync();

                return Ok("Printer serial parameter are deleted successfully from database.");
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
                var deviceName = DeviceEnum.Printer;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var printer = station.Devices[deviceName] as PrinterDevice;
                if (printer == null)
                {
                    return NotFound("Printer device not found.");
                }
                if (!printer.Config.IsEnabled)
                {
                    return NotFound("Printer device is disable.");
                }
                var deviceConfig = printer.Config;
                var deviceSerialParameters = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (deviceSerialParameters == null)
                {
                    return NotFound("Printer device serial parameters not found in DB.");
                }
                deviceConfig.serialConnectionParameter = deviceSerialParameters;
                var connectResult = printer.Connect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (printer.Config.IsConnected)
                {
                    return Ok("Printer device is connected");
                }
                else
                {
                    return Ok("Printer device can not connecte");
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
                var deviceName = DeviceEnum.Printer;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var printer = station.Devices[deviceName] as PrinterDevice;
                if (printer == null)
                {
                    return NotFound("Printer device not found.");
                }
                if (!printer.Config.IsEnabled)
                {
                    return NotFound("Printer device is disable.");
                }

                var connectResult = printer.Disconnect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (!printer.Config.IsConnected)   
                {
                    return Ok("Printer device is disconnected");
                }
                else
                {
                    return Ok("Printer device can not be disconnected");
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
                var deviceName = DeviceEnum.Printer;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var printer = station.Devices[deviceName] as PrinterDevice;
                if (printer == null)
                {
                    return NotFound("Printer device not found.");
                }
                printer.Enable();
                return Ok("Printer Device flag IsEnable : enabled");
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
                var deviceName = DeviceEnum.Printer;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var printer = station.Devices[deviceName] as PrinterDevice;
                if (printer == null)
                {
                    return NotFound("Printer device not found.");
                }
                printer.Disable();
                return Ok("Printer Device flag IsEnable : disabled");
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
                var printerDevice = station.Devices[DeviceEnum.Printer];
                var status = "";
                if (printerDevice.Config.IsEnabled)
                {
                    status = "Printer Device is enable";
                }
                else
                {
                    status = "Printer Device is disable";
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion

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
    }
}
