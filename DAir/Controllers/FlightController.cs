using Microsoft.AspNetCore.Mvc;
using DAir.Models; // Ensure this namespace matches your actual model's namespace
using DAir.Context;   // Replace with your actual DbContext's namespace
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly DAirDbContext _context; // Replace with your actual DbContext

        public FlightController(DAirDbContext context)
        {
            _context = context;
        }

        // GET: api/Flight
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            return await _context.Flights.ToListAsync();
        }

        // GET: api/Flight/5
        [HttpGet("{flightCode}")]
        public async Task<ActionResult<Flight>> GetFlight(string flightCode)
        {
            var flight = await _context.Flights
                .FirstOrDefaultAsync(f => f.FlightCode == flightCode);

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        // POST: api/Flight
        [HttpPost]
        public async Task<ActionResult<Flight>> PostFlight(Flight flight)
        {
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetFlight), new { flightCode = flight.FlightCode }, flight);
        }

        // PUT: api/Flight/5
        [HttpPut("{flightCode}")]
        public async Task<IActionResult> PutFlight(string flightCode, Flight flight)
        {
            if (flightCode != flight.FlightCode)
            {
                return BadRequest();
            }

            _context.Entry(flight).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightExists(flightCode))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        // GET: api/Flight/GetFlightInfo/SK935
        [HttpGet("GetFlightInfo/{flightCode}")]
        public async Task<ActionResult<Flight>> GetFlightInfo(string flightCode)
        {
            var flight = await _context.Flights
                .Where(f => f.FlightCode == flightCode)
                .FirstOrDefaultAsync();

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }
        // DELETE: api/Flight/5
        [HttpDelete("{flightCode}")]
        public async Task<IActionResult> DeleteFlight(string flightCode)
        {
            var flight = await _context.Flights
                .FirstOrDefaultAsync(f => f.FlightCode == flightCode);
            if (flight == null)
            {
                return NotFound();
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlightExists(string flightCode)
        {
            return _context.Flights.Any(f => f.FlightCode == flightCode);
        }
    }
}
