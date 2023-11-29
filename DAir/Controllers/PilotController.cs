﻿using Microsoft.AspNetCore.Mvc;
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
            _logger.LogInformation("Request received for GetPilots");
            return await _context.Pilots.ToListAsync();
        }

        // GET: api/Pilots/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pilot>> GetPilot(int id)
        {
            _logger.LogInformation("Request received for GetPilot with ID: {Id}", id);
            var pilot = await _context.Pilots.FindAsync(id);

            if (pilot == null)
            {
                _logger.LogWarning("Pilot not found with ID: {Id}", id);
                return NotFound();
            }

            return pilot;
        }

        // POST: api/Pilots
        [HttpPost]
        public async Task<ActionResult<Pilot>> PostPilot(Pilot pilot)
        {
            _logger.LogInformation("Request received for PostPilot");
            _context.Pilots.Add(pilot);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pilot created with ID: {PilotID}", pilot.PilotID);
            return CreatedAtAction("GetPilot", new { id = pilot.PilotID }, pilot);
        }

        // POST: api/Pilots/{id}/Ratings
        [HttpPost("{id}/Ratings")]
        public async Task<ActionResult<Rating>> PostRating(int id, Rating rating)
        {
            _logger.LogInformation("Request received for PostRating for Pilot ID: {Id}", id);
            var pilot = await _context.Pilots.FindAsync(id);

            if (pilot == null)
            {
                _logger.LogWarning("Pilot not found for rating with ID: {Id}", id);
                return NotFound("Pilot not found");
            }

            rating.RateeID = id;
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rating added for Pilot ID: {RateeID} by Rater ID: {RaterID}", rating.RateeID, rating.RaterID);
            return CreatedAtAction("GetRating", new { raterId = rating.RaterID, rateeId = rating.RateeID }, rating);
        }

        // PUT: api/Pilots/{id}/Ratings/{raterId}
        [HttpPut("{id}/Ratings/{raterId}")]
        public async Task<IActionResult> PutRating(int id, int raterId, Rating rating)
        {
            _logger.LogInformation("Request received for PutRating for Pilot ID: {Id} by Rater ID: {RaterId}", id, raterId);
            if (id != rating.RateeID || raterId != rating.RaterID)
            {
                _logger.LogWarning("PutRating received mismatched data");
                return BadRequest("Invalid rating data");
            }

            _context.Entry(rating).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Rating updated for Pilot ID: {RateeID} by Rater ID: {RaterID}", rating.RateeID, rating.RaterID);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!RatingExists(raterId, id))
                {
                    _logger.LogWarning("Rating not found for Pilot ID: {Id} and Rater ID: {RaterId}", id, raterId);
                    return NotFound("Rating not found");
                }
                else
                {
                    _logger.LogError(ex, "Error occurred during PutRating");
                    throw;
                }
            }
        }

        // DELETE: api/Pilots/{id}/Ratings/{raterId}
        [HttpDelete("{id}/Ratings/{raterId}")]
        public async Task<IActionResult> DeleteRating(int id, int raterId)
        {
            _logger.LogInformation("Request received for DeleteRating for Pilot ID: {Id} by Rater ID: {RaterId}", id, raterId);
            var rating = _context.Ratings.FirstOrDefault(r => r.RateeID == id && r.RaterID == raterId);

            if (rating == null)
            {
                _logger.LogWarning("Rating not found for deletion for Pilot ID: {Id} and Rater ID: {RaterId}", id, raterId);
                return NotFound("Rating not found");
            }

            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rating deleted for Pilot ID: {RateeID} by Rater ID: {RaterID}", rating.RateeID, rating.RaterID);
            return NoContent();
        }

        // PUT: api/Pilots/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPilot(int id, Pilot pilot)
        {
            _logger.LogInformation("Request received for PutPilot with ID: {Id}", id);
            if (id != pilot.PilotID)
            {
                _logger.LogWarning("PutPilot received mismatched ID for ID: {Id}", id);
                return BadRequest();
            }

            _context.Entry(pilot).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("Pilot updated with ID: {PilotID}", pilot.PilotID);
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PilotExists(id))
                {
                    _logger.LogWarning("Pilot not found for update with ID: {Id}", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error occurred during PutPilot");
                    throw;
                }
            }
        }

        // DELETE: api/Pilots/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePilot(int id)
        {
            _logger.LogInformation("Request received for DeletePilot with ID: {Id}", id);
            var pilot = await _context.Pilots.FindAsync(id);
            if (pilot == null)
            {
                _logger.LogWarning("Pilot not found for deletion with ID: {Id}", id);
                return NotFound();
            }

            _context.Pilots.Remove(pilot);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Pilot deleted with ID: {PilotID}", pilot.PilotID);
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
