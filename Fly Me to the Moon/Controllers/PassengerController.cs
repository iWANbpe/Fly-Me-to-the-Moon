using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Fly_Me_to_the_Moon.Services;
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetPassengers()
        {
            if (_context.Passenger == null)
            {
                return NotFound();
            }

            return await _context.Passenger.ToListAsync();
        }

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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

                    actionName: "GetPassenger",
                    routeValues: new { id = newPassenger.PassengerId },
                    value: newPassenger
                );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
            catch (ApplicationException ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An unexpected error occurred during deletion: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePassenger(int id, [FromBody] UpdatePassengerDto request)
        {

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
            catch (DbUpdateException ex)
            {
                return BadRequest($"Update failed due to data violation. Details: {ex.InnerException?.Message ?? ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

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
