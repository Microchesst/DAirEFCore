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
    public class ConflictsController : ControllerBase
    {
        private readonly DAirDbContext _context;

        public ConflictsController(DAirDbContext context)
        {
            _context = context;
        }

        // GET: api/Conflicts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Conflict>>> GetConflicts()
        {
            return await _context.Conflicts.ToListAsync();
        }

        // GET: api/Conflicts/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Conflict>> GetConflict(int id)
        {
            var conflict = await _context.Conflicts.FindAsync(id);

            if (conflict == null)
            {
                return NotFound();
            }

            return conflict;
        }

        // POST: api/Conflicts
        [HttpPost]
        public async Task<ActionResult<Conflict>> PostConflict(Conflict conflict)
        {
            _context.Conflicts.Add(conflict);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetConflict", new { id = conflict.ConflictID }, conflict);
        }

        // PUT: api/Conflicts/1
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConflict(int id, Conflict conflict)
        {
            if (id != conflict.ConflictID)
            {
                return BadRequest();
            }

            _context.Entry(conflict).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConflictExists(id))
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


        // DELETE: api/Conflicts/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConflict(int id)
        {
            var conflict = await _context.Conflicts.FindAsync(id);

            if (conflict == null)
            {
                return NotFound();
            }

            _context.Conflicts.Remove(conflict);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ConflictExists(int id)
        {
            return _context.Conflicts.Any(c => c.ConflictID == id);
        }
    }
}
