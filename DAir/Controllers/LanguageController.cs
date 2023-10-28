﻿using Microsoft.AspNetCore.Mvc;
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
    public class LanguagesController : ControllerBase
    {
        private readonly DAirDbContext _context;

        public LanguagesController(DAirDbContext context)
        {
            _context = context;
        }

        // GET: api/Languages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Languages>>> GetLanguages()
        {
            return await _context.Languages.ToListAsync();
        }

        // GET: api/Languages/5
        [HttpGet("{cabinMemberID}")]
        public async Task<ActionResult<IEnumerable<Languages>>> GetLanguagesForCabinMember(int cabinMemberID)
        {
            var languages = await _context.Languages
                .Where(l => l.CabinMemberID == cabinMemberID)
                .ToListAsync();

            if (languages == null || languages.Count == 0)
            {
                return NotFound();
            }

            return languages;
        }

        // POST: api/Languages
        [HttpPost]
        public async Task<ActionResult<Languages>> PostLanguages(Languages languages)
        {
            _context.Languages.Add(languages);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLanguagesForCabinMember", new { cabinMemberID = languages.CabinMemberID }, languages);
        }

        // PUT: api/Languages/5
        [HttpPut("{cabinMemberID}")]
        public async Task<IActionResult> PutLanguages(int cabinMemberID, Languages languages)
        {
            if (cabinMemberID != languages.CabinMemberID)
            {
                return BadRequest();
            }

            _context.Entry(languages).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LanguagesExist(cabinMemberID))
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

        // DELETE: api/Languages/5
        [HttpDelete("{cabinMemberID}")]
        public async Task<IActionResult> DeleteLanguages(int cabinMemberID)
        {
            var languages = await _context.Languages
                .Where(l => l.CabinMemberID == cabinMemberID)
                .ToListAsync();

            if (languages == null || languages.Count == 0)
            {
                return NotFound();
            }

            _context.Languages.RemoveRange(languages);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LanguagesExist(int cabinMemberID)
        {
            return _context.Languages.Any(l => l.CabinMemberID == cabinMemberID);
        }
    }
}