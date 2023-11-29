using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAir.Context;
using DAir.Models;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CrewController : ControllerBase
    {
        private readonly DAirDbContext _context;
        private readonly ILogger<CrewController> _logger;

        public CrewController(DAirDbContext context, ILogger<CrewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Crew
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Crew>>> GetCrews()
        {
            _logger.LogInformation("Request received for GetCrews");
            return await _context.Crews.ToListAsync();
        }

        // GET: api/Crew/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Crew>> GetCrew(int id)
        {
            _logger.LogInformation("Request received for GetCrew with ID: {Id}", id);
            var crew = await _context.Crews.FindAsync(id);

            if (crew == null)
            {
                _logger.LogWarning("Crew not found with ID: {Id}", id);
                return NotFound();
            }

            return crew;
        }

        // PUT: api/Crew/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCrew(int id, Crew crew)
        {
            _logger.LogInformation("Request received for PutCrew with ID: {Id}", id);
            if (id != crew.CrewID)
            {
                _logger.LogWarning("PutCrew received mismatched ID");
                return BadRequest();
            }

            _context.Entry(crew).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CrewExists(id))
                {
                    _logger.LogWarning("Crew not found during update with ID: {Id}", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error occurred during PutCrew");
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Crew
        [HttpPost]
        public async Task<ActionResult<Crew>> PostCrew(Crew crew)
        {
            _logger.LogInformation("Request received for PostCrew");
            _context.Crews.Add(crew);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCrew), new { id = crew.CrewID }, crew);
        }

        // DELETE: api/Crew/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrew(int id)
        {
            _logger.LogInformation("Request received for DeleteCrew with ID: {Id}", id);
            var crew = await _context.Crews.FindAsync(id);

            if (crew == null)
            {
                _logger.LogWarning("Crew not found for deletion with ID: {Id}", id);
                return NotFound();
            }

            _context.Crews.Remove(crew);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Crew deleted with ID: {Id}", id);
            return NoContent();
        }

        private bool CrewExists(int id)
        {
            return _context.Crews.Any(e => e.CrewID == id);
        }
    }
}
