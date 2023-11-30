using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAir.Models;
using DAir.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightSchedulesController : ControllerBase
    {
        private readonly DAirDbContext _context;
        private readonly ILogger<FlightSchedulesController> _logger;

        public FlightSchedulesController(DAirDbContext context, ILogger<FlightSchedulesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/FlightSchedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightSchedule>>> GetFlightSchedules()
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Get", Timestamp = timestamp };

            _logger.LogInformation("Get called {@Loginfo} ", logInfo);

            return await _context.FlightSchedules.ToListAsync();
        }

        // GET: api/FlightSchedules/ABC123
        [HttpGet("{flightCode}")]
        public async Task<ActionResult<FlightSchedule>> GetFlightSchedule(string flightCode)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Get", Timestamp = timestamp };

            _logger.LogInformation("Get called {@Loginfo} ", logInfo);

            var flightSchedule = await _context.FlightSchedules.FirstOrDefaultAsync(fs => fs.FlightCode == flightCode);

            if (flightSchedule == null)
            {
                _logger.LogWarning("FlightSchedule not found with FlightCode: {FlightCode}", flightCode);
                return NotFound();
            }

            return flightSchedule;
        }

        // POST: api/FlightSchedules
        [HttpPost]
        public async Task<ActionResult<FlightSchedule>> PostFlightSchedule(FlightSchedule flightSchedule)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Post", Timestamp = timestamp };

            _logger.LogInformation("Post called {@Loginfo} ", logInfo);

            _context.FlightSchedules.Add(flightSchedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlightSchedule), new { flightCode = flightSchedule.FlightCode }, flightSchedule);
        }

        // PUT: api/FlightSchedules/ABC123
        [HttpPut("{flightCode}")]
        public async Task<IActionResult> PutFlightSchedule(string flightCode, FlightSchedule flightSchedule)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Put", Timestamp = timestamp };

            _logger.LogInformation("Put called {@Loginfo} ", logInfo);

            if (flightCode != flightSchedule.FlightCode)
            {
                _logger.LogWarning("PutFlightSchedule received mismatched FlightCode");
                return BadRequest();
            }

            _context.Entry(flightSchedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!FlightScheduleExists(flightCode))
                {
                    _logger.LogWarning("FlightSchedule not found during update with FlightCode: {FlightCode}", flightCode);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error occurred during PutFlightSchedule");
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/FlightSchedules/ABC123
        [HttpDelete("{flightCode}")]
        public async Task<IActionResult> DeleteFlightSchedule(string flightCode)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Delete", Timestamp = timestamp };

            _logger.LogInformation("Delete called {@Loginfo} ", logInfo);

            var flightSchedule = await _context.FlightSchedules.FirstOrDefaultAsync(fs => fs.FlightCode == flightCode);

            if (flightSchedule == null)
            {
                _logger.LogWarning("FlightSchedule not found for deletion with FlightCode: {FlightCode}", flightCode);
                return NotFound();
            }

            _context.FlightSchedules.Remove(flightSchedule);
            await _context.SaveChangesAsync();

            _logger.LogInformation("FlightSchedule deleted with FlightCode: {FlightCode}", flightCode);
            return NoContent();
        }

        private bool FlightScheduleExists(string flightCode)
        {
            return _context.FlightSchedules.Any(fs => fs.FlightCode == flightCode);
        }
    }
}
