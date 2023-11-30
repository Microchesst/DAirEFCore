using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAir.Context;
using Microsoft.AspNetCore.Authorization;
using DAir.Models;

[ApiController]
[Route("api/dialects")]
[Authorize(Policy = "crewPolicy")]
public class DialectController : ControllerBase
{
    private readonly DAirDbContext _context;

    public DialectController(DAirDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Dialect>>> Get()
    {
        return await _context.Dialects.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Dialect>> GetById(int id)
    {
        var dialect = await _context.Dialects.FindAsync(id);

        if (dialect == null)
        {
            return NotFound();
        }

        return dialect;
    }

    [HttpPost]
    public async Task<ActionResult<Dialect>> Create(Dialect dialect)
    {
        _context.Dialects.Add(dialect);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = dialect.DialectID }, dialect);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Dialect updatedDialect)
    {
        if (id != updatedDialect.DialectID)
        {
            return BadRequest();
        }

        _context.Entry(updatedDialect).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Dialects.Any(e => e.DialectID == id))
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

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var dialect = await _context.Dialects.FindAsync(id);

        if (dialect == null)
        {
            return NotFound();
        }

        _context.Dialects.Remove(dialect);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
