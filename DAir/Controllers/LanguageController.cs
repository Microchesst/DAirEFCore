using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    [Authorize(Roles = "Crew")] // Restrict access to Cabin Crew
    public class LanguagesController : ControllerBase
    {
        private readonly DAirDbContext _context;
        private readonly ILogger<LanguagesController> _logger;

        public LanguagesController(DAirDbContext context, ILogger<LanguagesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Languages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Languages>>> GetLanguages()
        {
            _logger.LogInformation("Request received for GetLanguages");
            return await _context.Languages.ToListAsync();
        }

        // GET: api/Languages/5
        [HttpGet("{cabinMemberID}")]
        public async Task<ActionResult<IEnumerable<Languages>>> GetLanguagesForCabinMember(int cabinMemberID)
        {
            _logger.LogInformation("Request received for GetLanguagesForCabinMember with ID: {CabinMemberID}", cabinMemberID);
            var languages = await _context.Languages.Where(l => l.CabinMemberID == cabinMemberID).ToListAsync();

            if (languages == null || languages.Count == 0)
            {
                _logger.LogWarning("No languages found for CabinMemberID: {CabinMemberID}", cabinMemberID);
                return NotFound();
            }

            return languages;
        }

        // POST: api/Languages
        [HttpPost]
        public async Task<ActionResult<Languages>> PostLanguages(Languages languages)
        {
            _logger.LogInformation("Request received for PostLanguages");
            _context.Languages.Add(languages);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetLanguagesForCabinMember), new { cabinMemberID = languages.CabinMemberID }, languages);
        }

        // PUT: api/Languages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLanguages(int id, [FromBody] Languages languagesUpdate)
        {
            _logger.LogInformation("Request received for PutLanguages with ID: {Id}", id);
            if (id != languagesUpdate.ID)
            {
                _logger.LogWarning("PutLanguages received mismatched ID");
                return BadRequest();
            }

            var language = await _context.Languages.FindAsync(id);

            if (language == null)
            {
                _logger.LogWarning("Language not found with ID: {Id}", id);
                return NotFound();
            }

            language.Language = languagesUpdate.Language; // Update properties

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!LanguagesExist(id))
                {
                    _logger.LogWarning("Language update failed; not found with ID: {Id}", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error occurred during PutLanguages");
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Languages/5
        [HttpDelete("{cabinMemberID}")]
        public async Task<IActionResult> DeleteLanguages(int cabinMemberID)
        {
            _logger.LogInformation("Request received for DeleteLanguages with CabinMemberID: {CabinMemberID}", cabinMemberID);
            var languages = await _context.Languages.Where(l => l.CabinMemberID == cabinMemberID).ToListAsync();

            if (languages == null || languages.Count == 0)
            {
                _logger.LogWarning("No languages to delete for CabinMemberID: {CabinMemberID}", cabinMemberID);
                return NotFound();
            }

            _context.Languages.RemoveRange(languages);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Languages deleted for CabinMemberID: {CabinMemberID}", cabinMemberID);
            return NoContent();
        }

        private bool LanguagesExist(int cabinMemberID)
        {
            return _context.Languages.Any(l => l.CabinMemberID == cabinMemberID);
        }
    }
}
