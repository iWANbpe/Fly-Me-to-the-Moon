using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Fly_Me_to_the_Moon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengerController : ControllerBase
    {
        private readonly SpaceFlightContext _context;
        private readonly PassengerService _passengerService;

        public PassengerController(SpaceFlightContext context, PassengerService bookingService)
        {
            _context = context;
            _passengerService = bookingService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetPassengers()
        {
            if (_context.Passenger == null)
            {
                return NotFound();
            }

            return await _context.Passenger.ToListAsync();
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Passenger>> GetPassenger(int id)
        {
            if (_context.Passenger == null)
            {
                return NotFound();
            }

            var passenger = await _context.Passenger
                .Include(p => p.Insurance)
                .Include(p => p.FullHealthAnalysisResult)
                .FirstOrDefaultAsync(p => p.PassengerId == id);

            if (passenger == null)
            {
                return NotFound();
            }

            return passenger;
        }

        [Authorize(Roles = "Passenger")]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterPassenger([FromBody] PassengerRegistryRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newPassenger = await _passengerService.CreatePassengerWithDetails(request);

                
                return CreatedAtAction(

                    actionName: nameof(GetPassenger),
                    routeValues: new { id = newPassenger.PassengerId },
                    value: newPassenger
                );
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message.Contains("exists") || ex.Message.Contains("duplicate"))
                {
                    return Conflict(new { message = ex.Message });
                }

                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Passenger")]
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeletePassenger(int id)
        {
            try
            {
                bool wasDeleted = await _passengerService.DeletePassengerWithDetails(id);

                if (!wasDeleted)
                {
                    return NotFound($"Passenger with ID {id} not found.");
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ApplicationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred during deletion: {ex.Message}");
            }
        }

        [Authorize(Roles = "Passenger")]
        [HttpPost("{id}/insurance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddInsurance(int id, [FromBody] InsuranceDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.ExpireBy <= DateTime.UtcNow)
                return BadRequest(new { error = "Insurance policy is already expired." });

            try
            {
                var passenger = await _passengerService.AddInsuranceAsync(id, dto);
                return Ok(passenger);
            }
            catch (KeyNotFoundException ex) 
            { 
                return NotFound(ex.Message); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message); 
            }
        }

        [Authorize(Roles = "Passenger")]
        [HttpPost("{id}/health-analysis")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddHealthAnalysis(int id, [FromBody] FullHealthAnalysisResultDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (dto.ExpireBy <= DateTime.UtcNow)
                return BadRequest(new { error = "Health analysis is outdated." });

            try
            {
                var passenger = await _passengerService.AddHealthAnalysisAsync(id, dto);
                return Ok(passenger);
            }
            catch (KeyNotFoundException ex) 
            { 
                return NotFound(ex.Message); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, ex.Message); 
            }
        }

        [Authorize(Roles = "Passenger")]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassenger(int id, [FromBody] UpdatePassengerDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedPassenger = await _passengerService.UpdatePassengerAndLinked(id, request);
                return Ok(updatedPassenger);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                return BadRequest($"Update failed due to data violation. Details: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search/expired-insurance")]
        public async Task<IActionResult> GetPassengersByExpiredInsurance([FromQuery] ExpiredInsuranceFilterCriteriaDto filter, [FromQuery] int pageNumber = 1,[FromQuery] int pageSize = 15)
        {
            try
            {
                var passengers = await _passengerService.GetPassengersWithExpiredInsurance(
                    filter,
                    pageNumber,
                    pageSize
                );

                if (passengers == null || passengers.Count == 0)
                {
                    return NotFound("No passengers found with expired insurance before the specified date.");
                }

                return Ok(passengers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing the search request.", details = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("search/expired-analysis")]
        public async Task<IActionResult> GetPassengersWithExpiringHealthAnalysis([FromQuery] HealthAnalysisFilterCriteriaDto filter, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 15)
        {
            try
            {
                var passengers = await _passengerService.GetPassengersWithExpiringHealthAnalysis(
                    filter,
                    pageNumber,
                    pageSize
                );

                if (passengers == null || passengers.Count == 0)
                {
                    return NotFound("No passengers found with expired FHAR before the specified date.");
                }

                return Ok(passengers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred while processing the search request.", details = ex.Message });
            }
        }
    }
}
