using Microsoft.AspNetCore.Mvc;
using DAir.Models;
using DAir.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly DAirDbContext _context;
        private readonly ILogger<FlightController> _logger;

        public FlightController(DAirDbContext context, ILogger<FlightController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Flight
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Get", Timestamp = timestamp };

            _logger.LogInformation("Get called {@Loginfo} ", logInfo);
            
            return await _context.Flights.ToListAsync();
        }

        // GET: api/Flight/5
        [HttpGet("{flightCode}")]
        public async Task<ActionResult<Flight>> GetFlight(string flightCode)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Get", Timestamp = timestamp };

            _logger.LogInformation("Get called {@Loginfo} ", logInfo);

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightCode == flightCode);

            if (flight == null)
            {
                _logger.LogWarning("Flight not found with FlightCode: {FlightCode}", flightCode);
                return NotFound();
            }

            return flight;
        }

        // POST: api/Flight
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Post", Timestamp = timestamp };

            _logger.LogInformation("Post called {@Loginfo} ", logInfo);

            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlight), new { flightCode = flight.FlightCode }, flight);
        }

        // PUT: api/Flight/5
        [HttpPut("{flightCode}")]
        public async Task<IActionResult> PutFlight(string flightCode, Flight flight)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Put", Timestamp = timestamp };

            _logger.LogInformation("Put called {@Loginfo} ", logInfo);

            if (flightCode != flight.FlightCode)
            {
                _logger.LogWarning("PutFlight received mismatched FlightCode");
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!FlightExists(flightCode))
                {
                    _logger.LogWarning("Flight not found during update with FlightCode: {FlightCode}", flightCode);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error occurred during PutFlight");
                    throw;
                }
            }

            return NoContent();
        }

        // GET: api/Flight/GetFlightInfo/SK935
        [HttpGet("GetFlightInfo/{flightCode}")]
        public async Task<ActionResult<Flight>> GetFlightInfo(string flightCode)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Get", Timestamp = timestamp };

            _logger.LogInformation("Get called {@Loginfo} ", logInfo);

            var flight = await _context.Flights.Where(f => f.FlightCode == flightCode).FirstOrDefaultAsync();

            if (flight == null)
            {
                _logger.LogWarning("FlightInfo not found for FlightCode: {FlightCode}", flightCode);
                return NotFound();
            }

            return flight;
        }

        // DELETE: api/Flight/5
        [HttpDelete("{flightCode}")]
        public async Task<IActionResult> DeleteFlight(string flightCode)
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow);
            var logInfo = new { Operation = "Delete", Timestamp = timestamp };

            _logger.LogInformation("Delete called {@Loginfo} ", logInfo);

            var flight = await _context.Flights.FirstOrDefaultAsync(f => f.FlightCode == flightCode);

            if (flight == null)
            {
                _logger.LogWarning("Flight not found for deletion with FlightCode: {FlightCode}", flightCode);
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Flight deleted with FlightCode: {FlightCode}", flightCode);
            return NoContent();
        }

        private bool FlightExists(string flightCode)
        {
            return _context.Flights.Any(f => f.FlightCode == flightCode);
        }
    }
}
