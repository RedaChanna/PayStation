using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayStationSW.DataBase;
using PayStationSW.Devices;
using Microsoft.EntityFrameworkCore;

namespace PayStationSW.RESTAPI
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

        #region Entity
        [HttpPost("EnableEntity")]
        public async Task<IActionResult> EnableEntity()
        {

            try
            {
                var _deviceType = "1";
                var _deviceDescription = "Gryphone (lettore monete)";
                var _deviceEnable = "1";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "1")
                {
                    status = "Coin device enable entity in data base updated successfully";
                }
                else
                {
                    status = "Coin device enable entity in data base went wrong";
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
                var _deviceType = "1";
                var _deviceDescription = "Gryphone (lettore monete)";
                var _deviceEnable = "0";
                var deviceResult = await _deviceService.ManageDeviceAsync(_deviceType ?? "", _deviceDescription ?? "", _deviceEnable);
                var status = "";
                if (deviceResult.Enabled == "0")
                {
                    status = "Coin device disable entity in data base updated successfully";
                }
                else
                {
                    status = "Coin device disable entity in data base went wrong";
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
                var deviceType = "1";
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
                var deviceName = DeviceEnum.Coin;
                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    return BadRequest("Invalid device name or device not founded.");
                }

                var device = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (device == null)
                {
                    return NotFound("Serial parameter for coin device not found in DB.");
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
                deviceParameters.Device = DeviceEnum.Coin;
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

                return Ok("Coin serial parameters are updated successfully in data base.");
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

                var deviceName = DeviceEnum.Coin;
                // Cerca il dispositivo con il nome dato nel database
                var existingDevice = await _context.SerialConnectionParametersDB
                    .FirstOrDefaultAsync(x => x.Device == deviceName);

                if (existingDevice == null)
                {
                    return NotFound("Coin's serial parameter not found in database.");
                }

                // Rimuove il dispositivo dal database
                _context.SerialConnectionParametersDB.Remove(existingDevice);

                // Salva le modifiche nel database
                await _context.SaveChangesAsync();

                return Ok("Coin serial parameter are deleted successfully from database.");
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
                var deviceName = DeviceEnum.Coin;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var coin = station.Devices[deviceName] as CoinDevice;
                if (coin == null)
                {
                    return NotFound("Coin device not found.");
                }
                if (!coin.Config.IsEnabled)
                {
                    return NotFound("Coin device is disable.");
                }
                var deviceConfig = coin.Config;
                var deviceSerialParameters = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (deviceSerialParameters == null)
                {
                    return NotFound("Coin device serial parameters not found in DB.");
                }
                deviceConfig.serialConnectionParameter = deviceSerialParameters;
                var connectResult = coin.Connect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (coin.Config.IsConnected)
                {
                    return Ok("Coin device is connected");
                }
                else
                {
                    return Ok("Coin device can not connecte");
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
                var deviceName = DeviceEnum.Coin;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var coin = station.Devices[deviceName] as CoinDevice;
                if (coin == null)
                {
                    return NotFound("Coin device not found.");
                }
                if (!coin.Config.IsEnabled)
                {
                    return NotFound("Coin device is disable.");
                }

                var connectResult = coin.Disconnect();
                if (connectResult != "")
                {
                    return BadRequest(connectResult);
                }
                if (!coin.Config.IsConnected)
                {
                    return Ok("Coin device is disconnected");
                }
                else
                {
                    return Ok("Coin device can not be disconnected");
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
                var deviceName = DeviceEnum.Coin;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var coin = station.Devices[deviceName] as CoinDevice;
                if (coin == null)
                {
                    return NotFound("Coin device not found.");
                }
                coin.Enable();
                return Ok("Coin Device flag IsEnable : enabled");
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
                var deviceName = DeviceEnum.Coin;
                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    return NotFound("Station not found.");
                }
                var coin = station.Devices[deviceName] as CoinDevice;
                if (coin == null)
                {
                    return NotFound("Coin device not found.");
                }
                coin.Disable();
                return Ok("Coin Device flag IsEnable : disabled");
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
                var coinDevice = station.Devices[DeviceEnum.Coin];
                var status = "";
                if (coinDevice.Config.IsEnabled)
                {
                    status = "Coin Device is enable";
                }
                else
                {
                    status = "Coin Device is disable";
                }
                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        #endregion
        #region Presetting API
        [HttpPost("PreSettingSequence")]
        public async Task<IActionResult> PreSettingSequence()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (coinDevice.Config.IsConnected)
                {
                    string response = await coinDevice.PreSettingSequence();
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

        [HttpPost("Reset")]
        public async Task<IActionResult> Reset()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (coinDevice.Config.IsConnected)
                {
                    string response = await coinDevice.Reset();
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

        [HttpPost("SetUp")]
        public async Task<IActionResult> SetUp()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (coinDevice.Config.IsConnected)
                {
                    string response = await coinDevice.SetUp();
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

        [HttpPost("SetUpExpansion")]
        public async Task<IActionResult> SetUpExpansion()
        {
            try
            {
                Console.WriteLine("Expansion Set Up Coin Device called");
                var station = await StationManager.GetStationAsync(_context);
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (coinDevice.Config.IsConnected)
                {
                    string response = await coinDevice.SetUpExpansion();
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

        [HttpPost("SetUpFeature")]
        public async Task<IActionResult> SetUpFeature()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (coinDevice.Config.IsConnected)
                {
                    string response = await coinDevice.SetUpFeauture();
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
        #endregion
        #region General Commands
        [HttpPost("Inhibit")]
        public async Task<IActionResult> Inhibit()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (coinDevice.Config.IsConnected)
                {
                    string response = await coinDevice.InhibitionCommand();
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

        [HttpPost("Disinhibit")]
        public async Task<IActionResult> Disinhibit()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (coinDevice.Config.IsConnected)
                {
                    string response = await coinDevice.DisinhibitionCommand();
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

        [HttpPost("StartPolling")]
        public async Task<IActionResult> StartPolling()
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (coinDevice == null)
                {
                    return BadRequest(new { error = "The coin device is not present." });
                }
                if (!coinDevice.Config.IsConnected)
                {
                    return BadRequest(new { error = "The coin device is not a connected device." });
                }
                if (!coinDevice.Config.IsEnabled)
                {
                    return BadRequest(new { error = "The coin device is not a enable device." });

                }
                if (!coinDevice.Config.IsPreSetted)
                {
                    return BadRequest(new { error = "The coin device is not a pre-setted." });

                }
                if (!coinDevice.Config.IsInhibited)
                {
                    return BadRequest(new { error = "The coin device is not a inhibited." });
                }
                coinDevice.StartPolling();
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
                var coinDevice = station.Devices[DeviceEnum.Coin] as CoinDevice;
                if (coinDevice == null)
                {
                    return BadRequest(new { error = "The coin device is not present." });
                }
                coinDevice.StopPolling();    
                return Ok("Polling stoped correctly.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        #endregion




        /*
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