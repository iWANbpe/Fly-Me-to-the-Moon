using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class FullHealthAnalysisResultController : ControllerBase
{
    private readonly SpaceFlightContext _context;

    public FullHealthAnalysisResultController(SpaceFlightContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<FullHealthAnalysisResult>>> GetAnalysisResults()
    {
        return await _context.FullHealthAnalysisResult.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<FullHealthAnalysisResult>> GetAnalysisResult(int id)
    {
        var analysis = await _context.FullHealthAnalysisResult.FindAsync(id);

        if (analysis == null)
        {
            return NotFound();
        }

        return analysis;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FullHealthAnalysisResult>> PostAnalysis(FullHealthAnalysisResult analysis)
    {
        if (analysis == null || !ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {

            _context.FullHealthAnalysisResult.Add(analysis);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAnalysisResult), new { id = analysis.AnalysisId }, analysis);
        }
        catch (DbUpdateException ex)
        {

            string details = ex.InnerException != null ? ex.InnerException.Message : "Database error occurred (Details unavailable).";
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
    public async Task<IActionResult> DeleteAnalysisResult(int id)
    {
        var analysis = await _context.FullHealthAnalysisResult.FindAsync(id);

        if (analysis == null)
        {
            return NotFound();
        }

        try
        {
            _context.FullHealthAnalysisResult.Remove(analysis);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        catch (Exception ex)
        {
            string details = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting FullHealthAnalysisResult ID {id}. Details: {details}");
        }
    }
}