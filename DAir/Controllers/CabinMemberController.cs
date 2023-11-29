﻿using Microsoft.AspNetCore.Mvc;
using DAir.Models;
using DAir.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabinMembersController : ControllerBase
    {
        private readonly DAirDbContext _context;
        private readonly ILogger<CabinMembersController> _logger;

        public CabinMembersController(DAirDbContext context, ILogger<CabinMembersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/CabinMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CabinMember>>> GetCabinMembers()
        {
            _logger.LogInformation("Request received for GetCabinMembers");
            return await _context.CabinMembers.ToListAsync();
        }

        // GET: api/CabinMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CabinMember>> GetCabinMember(int id)
        {
            _logger.LogInformation("Request received for GetCabinMember with ID: {Id}", id);

            var cabinMember = await _context.CabinMembers.FirstOrDefaultAsync(cm => cm.CabinMemberID == id);

            if (cabinMember == null)
            {
                _logger.LogWarning("CabinMember not found with ID: {Id}", id);
                return NotFound();
            }

            return cabinMember;
        }

        // POST: api/CabinMembers
        [HttpPost]
        public async Task<ActionResult<CabinMember>> PostCabinMember(CabinMember cabinMember)
        {
            _logger.LogInformation("Request received for PostCabinMember");
            _context.CabinMembers.Add(cabinMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCabinMember), new { id = cabinMember.CabinMemberID }, cabinMember);
        }

        // PUT: api/CabinMembers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCabinMember(int id, CabinMember cabinMember)
        {
            _logger.LogInformation("Request received for PutCabinMember with ID: {Id}", id);

            if (id != cabinMember.CabinMemberID)
            {
                _logger.LogWarning("PutCabinMember received mismatched ID");
                return BadRequest();
            }

            _context.Entry(cabinMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CabinMemberExists(id))
                {
                    _logger.LogWarning("CabinMember not found during update with ID: {Id}", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, "Error occurred during PutCabinMember");
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/CabinMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabinMember(int id)
        {
            _logger.LogInformation("Request received for DeleteCabinMember with ID: {Id}", id);

            var cabinMember = await _context.CabinMembers.FirstOrDefaultAsync(cm => cm.CabinMemberID == id);

            if (cabinMember == null)
            {
                _logger.LogWarning("CabinMember not found for deletion with ID: {Id}", id);
                return NotFound();
            }

            _context.CabinMembers.Remove(cabinMember);
            await _context.SaveChangesAsync();

            _logger.LogInformation("CabinMember deleted with ID: {Id}", id);
            return NoContent();
        }

        private bool CabinMemberExists(int id)
        {
            return _context.CabinMembers.Any(cm => cm.CabinMemberID == id);
        }
    }
}
