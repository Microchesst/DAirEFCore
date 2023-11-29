using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using DAir.Models;
using DAir.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PilotsController : ControllerBase
    {
        private readonly DAirDbContext _context;

        // Inject Serilog's ILogger into the controller
        private readonly ILogger<PilotsController> _logger;

        public PilotsController(DAirDbContext context, ILogger<PilotsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Pilots
        [HttpGet]
        [Authorize(Policy = "adminPolicy")]
        public async Task<ActionResult<IEnumerable<Pilot>>> GetPilots()
        {
            return await _context.Pilots.ToListAsync();
        }

        // GET: api/Pilots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pilot>> GetPilot(int id)
        {
            var pilot = await _context.Pilots.FindAsync(id);

            if (pilot == null)
            {
                return NotFound();
            }

            return pilot;
        }

        // POST: api/Pilots
        [HttpPost]
        public async Task<ActionResult<Pilot>> PostPilot(Pilot pilot)
        {
            _context.Pilots.Add(pilot);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPilot", new { id = pilot.PilotID }, pilot);
        }

        // POST: api/Pilots/{id}/Ratings
        [HttpPost("{id}/Ratings")]
        public async Task<ActionResult<Rating>> PostRating(int id, Rating rating)
        {
            var pilot = await _context.Pilots.FindAsync(id);

            if (pilot == null)
            {
                return NotFound("Pilot not found");
            }

            // Set the ratee ID
            rating.RateeID = id;

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            // Log the POST request to MongoDB using Serilog
            _logger.LogInformation("Rating added for Pilot ID {PilotID} by Rater ID {RaterID}", rating.RateeID, rating.RaterID);


            return CreatedAtAction("GetRating", new { raterId = rating.RaterID, rateeId = rating.RateeID }, rating);
        }

        // PUT: api/Pilots/{id}/Ratings/{raterId}
        [HttpPut("{id}/Ratings/{raterId}")]
        public async Task<IActionResult> PutRating(int id, int raterId, Rating rating)
        {
            if (id != rating.RateeID || raterId != rating.RaterID)
            {
                return BadRequest("Invalid rating data");
            }

            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                // Log the PUT request to MongoDB using Serilog
                _logger.LogInformation("Rating added for Pilot ID {PilotID} by Rater ID {RaterID}", rating.RateeID, rating.RaterID);


                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(raterId, id))
                {
                    return NotFound("Rating not found");
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE: api/Pilots/{id}/Ratings/{raterId}
        [HttpDelete("{id}/Ratings/{raterId}")]
        public async Task<IActionResult> DeleteRating(int id, int raterId)
        {
            var rating = _context.Ratings.FirstOrDefault(r => r.RateeID == id && r.RaterID == raterId);

            if (rating == null)
            {
                return NotFound("Rating not found");
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            // Log the DELETE request to MongoDB using Serilog
            _logger.LogInformation("Rating deleted for Pilot ID {PilotID} by Rater ID {RaterID}", rating.RateeID, rating.RaterID);

            return NoContent();
        }

        // PUT: api/Pilots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPilot(int id, Pilot pilot)
        {
            if (id != pilot.PilotID)
            {
                return BadRequest();
            }

            _context.Entry(pilot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PilotExists(id))
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

        // DELETE: api/Pilots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePilot(int id)
        {
            var pilot = await _context.Pilots.FindAsync(id);
            if (pilot == null)
            {
                return NotFound();
            }

            _context.Pilots.Remove(pilot);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RatingExists(int raterId, int rateeId)
        {
            return _context.Ratings.Any(r => r.RaterID == raterId && r.RateeID == rateeId);
        }

        private bool PilotExists(int id)
        {
            return _context.Pilots.Any(e => e.PilotID == id);
        }

    }
}