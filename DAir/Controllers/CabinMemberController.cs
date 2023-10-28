using Microsoft.AspNetCore.Mvc;
using DAir.Models;
using DAir.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DAir.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CabinMembersController : ControllerBase
    {
        private readonly DAirDbContext _context;

        public CabinMembersController(DAirDbContext context)
        {
            _context = context;
        }

        // GET: api/CabinMembers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CabinMember>>> GetCabinMembers()
        {
            return await _context.CabinMembers.ToListAsync();
        }

        // GET: api/CabinMembers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CabinMember>> GetCabinMember(int id)
        {
            var cabinMember = await _context.CabinMembers
                .FirstOrDefaultAsync(cm => cm.CabinMemberID == id);

            if (cabinMember == null)
            {
                return NotFound();
            }

            return cabinMember;
        }

        // POST: api/CabinMembers
        [HttpPost]
        public async Task<ActionResult<CabinMember>> PostCabinMember(CabinMember cabinMember)
        {
            _context.CabinMembers.Add(cabinMember);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCabinMember), new { id = cabinMember.CabinMemberID }, cabinMember);
        }

        // PUT: api/CabinMembers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCabinMember(int id, CabinMember cabinMember)
        {
            if (id != cabinMember.CabinMemberID)
            {
                return BadRequest();
            }

            _context.Entry(cabinMember).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CabinMemberExists(id))
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

        // DELETE: api/CabinMembers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCabinMember(int id)
        {
            var cabinMember = await _context.CabinMembers
                .FirstOrDefaultAsync(cm => cm.CabinMemberID == id);
            if (cabinMember == null)
            {
                return NotFound();
            }

            _context.CabinMembers.Remove(cabinMember);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CabinMemberExists(int id)
        {
            return _context.CabinMembers.Any(cm => cm.CabinMemberID == id);
        }
    }
}
