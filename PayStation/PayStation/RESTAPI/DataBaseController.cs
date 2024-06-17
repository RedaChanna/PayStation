using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayStationSW.DataBase;

namespace PayStationSW.RESTAPI
{
    [Route("api/DataBase")]
    [ApiController]
    public class DataBaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public DataBaseController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DataBase/Movements
        [HttpGet("Movements")]
        public async Task<ActionResult<IEnumerable<MovementDB>>> GetMovements()
        {
            try
            {
                var movements = await _context.MovementsDB.ToListAsync();
                return Ok(movements);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
            
        // GET: api/DataBase/IngenicoPosMovements
        [HttpGet("IngenicoPosMovements")]
        public async Task<ActionResult<IEnumerable<IngenicoPosMovementDB>>> GetIngenicoPosMovements()
        {
            try
            {
                var ingenicoPosMovements = await _context.IngenicoPosMovementsDB.ToListAsync();
                return Ok(ingenicoPosMovements);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/DataBase/Alarms
        [HttpGet("Alarms")]
        public async Task<ActionResult<IEnumerable<AlarmsDB>>> GetAlarms()
        {
            try
            {
                var alarms = await _context.AlarmsDB.ToListAsync();
                return Ok(alarms);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/DataBase/CashClosureReceipts
        [HttpGet("CashClosureReceipts")]
        public async Task<ActionResult<IEnumerable<CashClosureReceiptDB>>> GetCashClosureReceipts()
        {
            try
            {
                var cashClosureReceipts = await _context.CashClosureReceiptsDB.ToListAsync();
                return Ok(cashClosureReceipts);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/DataBase/PartialCashClosures
        [HttpGet("PartialCashClosures")]
        public async Task<ActionResult<IEnumerable<PartialCashClosureDB>>> GetPartialCashClosures()
        {
            try
            {
                var partialCashClosures = await _context.PartialCashClosuresDB.ToListAsync();
                return Ok(partialCashClosures);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/DataBase/CashDetails
        [HttpGet("CashDetails")]
        public async Task<ActionResult<IEnumerable<CashDetailDB>>> GetCashDetails()
        {
            try
            {
                var cashDetails = await _context.CashDetailsDB.ToListAsync();
                return Ok(cashDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: dotnet ef database update

        [HttpGet("DevicesEntity")]
        public async Task<ActionResult<IEnumerable<DeviceEntityDB>>> GetDevices()
        {
            try
            {
                var devices = await _context.DevicesDB.ToListAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/DataBase/Parameters
        [HttpGet("Parameters")]
        public async Task<ActionResult<IEnumerable<ParameterDB>>> GetParameters()
        {
            try
            {
                var parameters = await _context.ParametersDB.ToListAsync();
                return Ok(parameters);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
            
        // GET: api/DataBase/SerialConnectionParameters
        [HttpGet("SerialConnectionParameters")]
        public async Task<ActionResult<IEnumerable<SerialConnectionParameterDB>>> GetSerialConnectionParameters()
        {
            try
            {
                var serialConnectionParameters = await _context.SerialConnectionParametersDB.ToListAsync();
                return Ok(serialConnectionParameters);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/DataBase/SerialConnectionSetting
        [HttpGet("SerialConnectionSetting")]
        public async Task<ActionResult<IEnumerable<SerialConnectionSettingDB>>> GetSerialConnectionSetting()
        {
            try
            {
                var serialConnectionSetting = await _context.SerialConnectionSettingDB.ToListAsync();
                return Ok(serialConnectionSetting);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/DataBase/TicketLayout
        [HttpGet("TicketLayout")]
        public async Task<ActionResult<IEnumerable<TicketLayoutDB>>> GetTicketLayout()
        {
            try
            {
                var serialConnectionSetting = await _context.TicketLayoutDB.ToListAsync();
                return Ok(serialConnectionSetting);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: api/DataBase/TextPrinterObjects
        [HttpGet("TextPrinterObjects")]
        public async Task<ActionResult<IEnumerable<TicketLayoutDB>>> GetTextPrinterObjects()
        {
            try
            {
                var serialConnectionSetting = await _context.TextPrinterObjectsDB.ToListAsync();
                return Ok(serialConnectionSetting);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
