using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAir.Models;
using DAir.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightSchedulesController : ControllerBase
    {
        private readonly DAirDbContext _context;

        public FlightSchedulesController(DAirDbContext context)
        {
            _context = context;
        }

        // GET: api/FlightSchedules
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FlightSchedule>>> GetFlightSchedules()
        {
            return await _context.FlightSchedules.ToListAsync();
        }

        // GET: api/FlightSchedules/ABC123
        [HttpGet("{flightCode}")]
        public async Task<ActionResult<FlightSchedule>> GetFlightSchedule(string flightCode)
        {
            var flightSchedule = await _context.FlightSchedules
                .FirstOrDefaultAsync(fs => fs.FlightCode == flightCode);

            if (flightSchedule == null)
            {
                return NotFound();
            }

            return flightSchedule;
        }

        // POST: api/FlightSchedules
        [HttpPost]
        public async Task<ActionResult<FlightSchedule>> PostFlightSchedule(FlightSchedule flightSchedule)
        {
            _context.FlightSchedules.Add(flightSchedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFlightSchedule", new { flightCode = flightSchedule.FlightCode }, flightSchedule);
        }

        // PUT: api/FlightSchedules/ABC123
        [HttpPut("{flightCode}")]
        public async Task<IActionResult> PutFlightSchedule(string flightCode, FlightSchedule flightSchedule)
        {
            if (flightCode != flightSchedule.FlightCode)
            {
                return BadRequest();
            }

            _context.Entry(flightSchedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FlightScheduleExists(flightCode))
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

        // DELETE: api/FlightSchedules/ABC123
        [HttpDelete("{flightCode}")]
        public async Task<IActionResult> DeleteFlightSchedule(string flightCode)
        {
            var flightSchedule = await _context.FlightSchedules
                .FirstOrDefaultAsync(fs => fs.FlightCode == flightCode);
            if (flightSchedule == null)
            {
                return NotFound();
            }

            _context.FlightSchedules.Remove(flightSchedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FlightScheduleExists(string flightCode)
        {
            return _context.FlightSchedules.Any(fs => fs.FlightCode == flightCode);
        }
    }
}
