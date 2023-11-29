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
    public class ConflictsController : ControllerBase
    {
        private readonly DAirDbContext _context;
        private readonly ILogger<ConflictsController> _logger;

        public ConflictsController(DAirDbContext context, ILogger<ConflictsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Conflicts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Conflict>>> GetConflicts()
        {
            _logger.LogInformation("Request received for GetConflicts");
            return await _context.Conflicts.ToListAsync();
        }

        // GET: api/Conflicts/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Conflict>> GetConflict(int id)
        {
            _logger.LogInformation("Request received for GetConflict with ID: {Id}", id);
            var conflict = await _context.Conflicts.FindAsync(id);

            if (conflict == null)
            {
                _logger.LogWarning("Conflict not found with ID: {Id}", id);
                return NotFound();
            }

            return conflict;
        }

        // POST: api/Conflicts
        [HttpPost]
        public async Task<ActionResult<Conflict>> PostConflict(Conflict conflict)
        {
            _logger.LogInformation("Request received for PostConflict");
            _context.Conflicts.Add(conflict);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetConflict), new { id = conflict.ConflictID }, conflict);
        }

        // PUT: api/Conflicts/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConflict(int id, Conflict conflict)
        {
            _logger.LogInformation("Request received for PutConflict with ID: {Id}", id);
            if (id != conflict.ConflictID)
            {
                _logger.LogWarning("PutConflict received mismatched ID");
                return BadRequest();
            }

            _context.Entry(conflict).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ConflictExists(id))
                {
                    _logger.LogWarning("Conflict not found during update with ID: {Id}", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error occurred during PutConflict");
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Conflicts/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConflict(int id)
        {
            _logger.LogInformation("Request received for DeleteConflict with ID: {Id}", id);
            var conflict = await _context.Conflicts.FindAsync(id);

            if (conflict == null)
            {
                _logger.LogWarning("Conflict not found for deletion with ID: {Id}", id);
                return NotFound();
            }

            _context.Conflicts.Remove(conflict);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Conflict deleted with ID: {Id}", id);
            return NoContent();
        }

        private bool ConflictExists(int id)
        {
            return _context.Conflicts.Any(c => c.ConflictID == id);
        }
    }
}
