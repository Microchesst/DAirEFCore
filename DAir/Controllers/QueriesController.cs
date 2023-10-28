using DAir.Context;
using DAir.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private readonly DAirDbContext _context;

        public QueriesController(DAirDbContext context)
        {
            _context = context;
        }

        // Add your query methods here
        // Example: Get flight details by flight code
        [HttpGet("GetFlightDetails/{flightCode}")]
        public async Task<ActionResult<Flight>> GetFlightDetails(string flightCode)
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

        // Other query methods...
    }
}