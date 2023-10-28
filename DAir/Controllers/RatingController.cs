using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAir.Context;
using DAir.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly DAirDbContext _context;

        public RatingsController(DAirDbContext context)
        {
            _context = context;
        }

        // GET: api/Ratings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rating>>> GetRatings()
        {
            return await _context.Ratings.ToListAsync();
        }

        // GET: api/Ratings/5/10
        [HttpGet("{raterId}/{rateeId}")]
        public async Task<ActionResult<Rating>> GetRating(int raterId, int rateeId)
        {
            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.RaterID == raterId && r.RateeID == rateeId);

            if (rating == null)
            {
                return NotFound();
            }

            return rating;
        }

        // POST: api/Ratings
        [HttpPost]
        public async Task<ActionResult<Rating>> PostRating(Rating rating)
        {
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRating", new { raterId = rating.RaterID, rateeId = rating.RateeID }, rating);
        }

        // PUT: api/Ratings/5/10
        [HttpPut("{raterId}/{rateeId}")]
        public async Task<IActionResult> PutRating(int raterId, int rateeId, Rating rating)
        {
            if (raterId != rating.RaterID || rateeId != rating.RateeID)
            {
                return BadRequest();
            }

            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RatingExists(raterId, rateeId))
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

        // DELETE: api/Ratings/5/10
        [HttpDelete("{raterId}/{rateeId}")]
        public async Task<IActionResult> DeleteRating(int raterId, int rateeId)
        {
            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.RaterID == raterId && r.RateeID == rateeId);

            if (rating == null)
            {
                return NotFound();
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RatingExists(int raterId, int rateeId)
        {
            return _context.Ratings.Any(r => r.RaterID == raterId && r.RateeID == rateeId);
        }
    }
}
