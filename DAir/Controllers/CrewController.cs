using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAir.Context;
using DAir.Models;
using Microsoft.AspNetCore.Authorization;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "crewPolicy")]

    public class CrewController : ControllerBase
    {
        private readonly DAirDbContext _context;

        public CrewController(DAirDbContext context)
        {
            _context = context;
        }

        // GET: api/Crew
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Crew>>> GetCrews()
        {
            return await _context.Crews.ToListAsync();
        }

        // GET: api/Crew/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Crew>> GetCrew(int id)
        {
            var crew = await _context.Crews.FindAsync(id);

            if (crew == null)
            {
                return NotFound();
            }

            return crew;
        }

        // PUT: api/Crew/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCrew(int id, Crew crew)
        {
            if (id != crew.CrewID)
            {
                return BadRequest();
            }

            _context.Entry(crew).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CrewExists(id))
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

        // POST: api/Crew
        [HttpPost]
        public async Task<ActionResult<Crew>> PostCrew(Crew crew)
        {
            _context.Crews.Add(crew);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCrew", new { id = crew.CrewID }, crew);
        }

        // DELETE: api/Crew/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCrew(int id)
        {
            var crew = await _context.Crews.FindAsync(id);
            if (crew == null)
            {
                return NotFound();
            }

            _context.Crews.Remove(crew);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CrewExists(int id)
        {
            return _context.Crews.Any(e => e.CrewID == id);
        }
    }
}
