using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO.Ports;
using System.Data;
using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PayStationName.DataBase;
using System.ComponentModel.DataAnnotations.Schema;
using PayStationName.Devices;
using static System.Collections.Specialized.BitVector32;
using Microsoft.Extensions.Logging;  // Assicurati di includere questa direttiva

namespace PayStationName.RESTAPI
{

    [Route("api/SerialConnection")]
    [ApiController]
    public class SerialConnectionController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly ILogger<SerialConnectionController> _logger; // Aggiungi questa variabile per il logger

        public SerialConnectionController(ApplicationDbContext context, ILogger<SerialConnectionController> logger)
        {
            _context = context;
            _logger = logger; // Inizializza il logger
        }

        // GET: api/SerialConnection/SerialPortsAvailable
        [HttpGet("SerialPortsAvailable")]
        public IActionResult SerialPortsAvailable()
        {
            try
            {
                // Gets the list of available serial ports on the system
                string[] ports = SerialPort.GetPortNames();

                // Sorts the array for better readability, if needed
                Array.Sort(ports);

                // Returns the list of serial ports as an array of strings
                return Ok(ports);
            }
            catch (Exception ex)
            {
                // Returns an error message in case of an exception
                return StatusCode(500, new { errore = ex.Message });
            }
        }









        [HttpPost("ConnectDevice")]
        public async Task<IActionResult> ConnectDevice([FromBody] SerialConnectionParameterDB serialParameter)
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                bool connected = false;
                if (serialParameter.LastPortName != null)
                {
                    switch (serialParameter.Device)
                    {
                        case DeviceEnum.Cash:
                            station.Devices[DeviceEnum.Cash].Config.serialConnectionParameter = serialParameter;
                            station.Devices[DeviceEnum.Cash].Disconnect();
                            station.Devices[DeviceEnum.Cash].Connect();
                            break;
                        case DeviceEnum.Coin:
                            station.Devices[DeviceEnum.Coin].Config.serialConnectionParameter = serialParameter;
                            station.Devices[DeviceEnum.Coin].Disconnect();
                            station.Devices[DeviceEnum.Coin].Connect();
                            break;
                        case DeviceEnum.Pos:
                            station.Devices[DeviceEnum.Pos].Config.serialConnectionParameter = serialParameter;
                            station.Devices[DeviceEnum.Pos].Disconnect();
                            station.Devices[DeviceEnum.Pos].Connect();
                            break;
                        case DeviceEnum.Printer:
                            station.Devices[DeviceEnum.Printer].Config.serialConnectionParameter = serialParameter;
                            station.Devices[DeviceEnum.Printer].Disconnect();
                            station.Devices[DeviceEnum.Printer].Connect();
                            break;
                        default:
                            connected = false;
                            return BadRequest(new { errore = "Tipo di dispositivo non supportato." });
                    }

                    if (serialParameter.Device.HasValue)
                    {
                        connected = station.Devices[serialParameter.Device.Value].Config.IsConnected;
                    }
                    else
                    {
                        connected = false;
                    }
                    if (connected)
                    {
                        return Ok(new { stato = "Connesso" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errore = ex.Message });
            }
            return BadRequest(new { errore = "Errore sconosciuto." });
        }

        /*
        // POST: api/SerialConnection/ConnectDevice
        [HttpPost("ConnectDevice")]

        public async Task<IActionResult> ConnectDevice([FromBody] SerialConnectionParameterDB serialParameter)
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);

                bool connected = false;
                if (serialParameter.Device != null &&
                    serialParameter.LastPortName != null)
                {
                    switch (serialParameter.Device.ToUpper())
                    {
                        case "CASH":
                            station.CashDevice.serialConnectionParamter = serialParameter;
                            station.CashDevice.Disconnect();
                            station.CashDevice.Connect();
                            if (station.CashDevice.IsConnected)
                            {
                                connected = true;
                                Console.WriteLine(serialParameter.Device + " è connesso");
                            }
                            break;
                        case "COIN":
                            station.CoinDevice.serialConnectionParamter = serialParameter;
                            station.CoinDevice.Disconnect();
                            station.CoinDevice.Connect();
                            if (station.CoinDevice.IsConnected)
                            {
                                connected = true;
                                Console.WriteLine(serialParameter.Device + " è connesso");
                            }
                            break;
                        case "POS":
                            station.POSDevice.serialConnectionParamter = serialParameter;
                            station.POSDevice.Disconnect();
                            station.POSDevice.Connect();
                            if (station.POSDevice.IsConnected)
                            {
                                connected = true;
                                Console.WriteLine(serialParameter.Device + " è connesso");
                            }
                            break;
                        case "PRINTER":
                            station.PrinterDevice.serialConnectionParamter = serialParameter;
                            station.PrinterDevice.Disconnect();
                            station.PrinterDevice.Connect();
                            if (station.PrinterDevice.IsConnected)
                            {
                                connected = true;
                                Console.WriteLine(serialParameter.Device + " è connesso");
                            }
                            break;
                        // Add other cases as needed
                        default:
                            connected = false;
                            return BadRequest(new { errore = "Tipo di dispositivo non supportato." });
                    }
                    // Set common parameters and connect
                    if (connected)
                    {
                        return Ok(new { stato = "Connesso" });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { errore = ex.Message });
            }
            return BadRequest(new { errore = "Errore sconosciuto." });
        }

        */

        // POST: api/SerialConnection/ConnectDeviceRegistred
        [HttpPost("ConnectDeviceRegistred")]
        public async Task<IActionResult> ConnectDeviceRegistred(DeviceEnum deviceName)
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    return BadRequest("Invalid device name.");
                }
                var deviceSerialParameters = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                if (deviceSerialParameters == null)
                {
                    return NotFound("Device not found in DB.");
                }
                else
                {
                    station.Devices[deviceName].Config.serialConnectionParameter = deviceSerialParameters;
                    station.Devices[deviceName].Disconnect();
                    station.Devices[deviceName].Connect();
                    return Ok("Device Connected");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/SerialConnection/DisconnectDeviceRegistred
        [HttpPost("DisconnectDevice")]
        public async Task<IActionResult> DisconnectDeviceRegistred(DeviceEnum deviceName)
        {
            try
            {
                var station = await StationManager.GetStationAsync(_context);
                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    return BadRequest("Invalid device name.");
                }
                station.Devices[deviceName].Disconnect();
                return Ok("Device Disconnected");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }








         






        // POST: api/SerialConnection/ConnectDisconnectDevice
        [HttpPost("ConnectDisconnectDevice")]
        public async Task<IActionResult> ConnectDisconnectDevice([FromBody] ConnectionObj connectionObj)
        {
            try
            {
                Console.WriteLine("Disconnect and connect Device API called");
                if (connectionObj == null)
                {
                    _logger.LogError("Received null connection object.");
                    return BadRequest("Connection object must not be null.");
                }

                _logger.LogInformation($"Received object: {Newtonsoft.Json.JsonConvert.SerializeObject(connectionObj)}");

                if (!connectionObj.deviceName.HasValue)
                {
                    _logger.LogError("Device name is null.");
                    return BadRequest("Device name must not be null.");
                }

                var deviceName = connectionObj.deviceName.Value;

                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    _logger.LogError("Invalid device name.");
                    return BadRequest("Invalid device name.");
                }

                var station = await StationManager.GetStationAsync(_context);
                if (station == null)
                {
                    _logger.LogError("Station not found.");
                    return NotFound("Station not found.");
                }

                if (!station.Devices[deviceName].Config.IsEnabled)
                {
                    _logger.LogError("Device is not enable.");
                    return NotFound("Device is not enable.");
                }

                var deviceConfig = station.Devices[deviceName].Config;

                if (!connectionObj.deviceNameOrSerialData)
                {
                    var deviceSerialParameters = await _context.SerialConnectionParametersDB
                        .Where(x => x.Device == deviceName)
                        .FirstOrDefaultAsync();

                    if (deviceSerialParameters == null)
                    {
                        _logger.LogError("Device not found in DB.");
                        return NotFound("Device not found in DB.");
                    }

                    deviceConfig.serialConnectionParameter = deviceSerialParameters;
                }
                else
                {
                    if (connectionObj.port == null)
                    {
                        _logger.LogError("Serial parameters are null.");
                        return BadRequest("Serial parameters must not be null when using direct data.");
                    }

                    deviceConfig.serialConnectionParameter = connectionObj.port;
                }

                if (connectionObj.connectDisconnect)
                {
                    station.Devices[deviceName].Connect();
                    _logger.LogInformation("Device connected.");
                    return Ok("Device Connected");
                }
                else
                {
                    station.Devices[deviceName].Disconnect();
                    _logger.LogInformation("Device disconnected.");
                    return Ok("Device Disconnected");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during API call: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/SerialConnection/GetDeviceConnectionParameter
        [HttpGet("GetDeviceConnectionParameter")]
        public async Task<ActionResult<SerialConnectionParameterDB>> GetDeviceConnectionParameter(DeviceEnum deviceName)
        {
            // Aggiungi un log per stampare il nome del dispositivo ricevuto
            Console.WriteLine("Received device name: " + deviceName);

            try
            {
                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    return BadRequest("Invalid device name.");
                }

                var device = await _context.SerialConnectionParametersDB
                    .Where(x => x.Device == deviceName)
                    .FirstOrDefaultAsync();
                Console.WriteLine($"Query Result: {device?.ToString() ?? "No device found"}");
                if (device == null)
                {
                    return NotFound("Device not found.");
                }

                return Ok(device);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: api/SerialConnection/SetDeviceConnectionParameters
        [HttpPost("SetDeviceConnectionParameters")]
        public async Task<ActionResult> SetDeviceConnectionParameters([FromBody] SerialConnectionParameterDB deviceParameters)
        {
            try
            {
                Console.WriteLine(deviceParameters.Device);
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

                return Ok("Device parameters updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // DELET: api/SerialConnection/DeleteDeviceConnectionParameters
        [HttpDelete("DeleteDeviceConnectionParameters/{deviceName}")]
        public async Task<IActionResult> DeleteDeviceConnectionParameters(DeviceEnum deviceName)
        {
            try
            {
                // Verifica se il valore fornito è un membro valido dell'enum DeviceEnum
                if (!Enum.IsDefined(typeof(DeviceEnum), deviceName))
                {
                    return BadRequest("Invalid device name.");
                }
                // Cerca il dispositivo con il nome dato nel database
                var existingDevice = await _context.SerialConnectionParametersDB
                    .FirstOrDefaultAsync(x => x.Device == deviceName);

                if (existingDevice == null)
                {
                    return NotFound("Device not found.");
                }

                // Rimuove il dispositivo dal database
                _context.SerialConnectionParametersDB.Remove(existingDevice);

                // Salva le modifiche nel database
                await _context.SaveChangesAsync();

                return Ok("Device deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log dell'errore (aggiungi il logging appropriato qui)
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}