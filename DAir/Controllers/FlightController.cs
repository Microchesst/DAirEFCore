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
            _logger.LogInformation("Request received for GetFlights");
            return await _context.Flights.ToListAsync();
        }

        // GET: api/Flight/5
        [HttpGet("{flightCode}")]
        public async Task<ActionResult<Flight>> GetFlight(string flightCode)
        {
            _logger.LogInformation("Request received for GetFlight with FlightCode: {FlightCode}", flightCode);
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
            _logger.LogInformation("Request received for PostFlight");
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlight), new { flightCode = flight.FlightCode }, flight);
        }

        // PUT: api/Flight/5
        [HttpPut("{flightCode}")]
        public async Task<IActionResult> PutFlight(string flightCode, Flight flight)
        {
            _logger.LogInformation("Request received for PutFlight with FlightCode: {FlightCode}", flightCode);
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
            _logger.LogInformation("Request received for GetFlightInfo with FlightCode: {FlightCode}", flightCode);
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
            _logger.LogInformation("Request received for DeleteFlight with FlightCode: {FlightCode}", flightCode);
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
