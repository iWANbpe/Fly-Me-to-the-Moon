using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class InsuranceController : ControllerBase
{
    private readonly SpaceFlightContext _context;

    public InsuranceController(SpaceFlightContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Insurance>>> GetInsurances()
    {
        return await _context.Insurance.ToListAsync();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Insurance>> PostInsurance(Insurance insurance)
    {
        if (insurance == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            _context.Insurance.Add(insurance);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(PostInsurance), new { id = insurance.InsuranceId }, insurance);
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            string details = ex.InnerException != null ? ex.InnerException.Message : "Database error occurred.";
            return StatusCode(StatusCodes.Status500InternalServerError, $"DB Error: {details}");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"General Error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteInsurance(int id)
    {
        var insurance = await _context.Insurance.FindAsync(id);

        if (insurance == null)
        {
            return NotFound();
        }

        try
        {
            _context.Insurance.Remove(insurance);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        catch (Exception ex)
        {
            string details = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting Insurance ID {id}. Details: {details}");
        }
    }
}