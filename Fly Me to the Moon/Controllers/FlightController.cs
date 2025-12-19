using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Fly_Me_to_the_Moon.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlightController : ControllerBase
    {
        private readonly FlightService _flightService;
        private readonly SpaceFlightContext _context;

        public FlightController(FlightService flightService, SpaceFlightContext context)
        {
            _flightService = flightService;
            _context = context;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FlightDetailsDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFlight([FromBody] FlightCreationDto flightDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newFlight = await _flightService.AddFlight(flightDto);

                return CreatedAtAction(
                    nameof(GetFlight),
                    new { id = newFlight.FlightId },
                    newFlight);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "There was an issue while adding flight", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Flight>>> GetFlights()
        {
            if (_context.Flight == null)
            {
                return NotFound();
            }

            return await _context.Flight.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Flight>> GetFlight(int id)
        {
            if (_context.Flight == null)
            {
                return NotFound();
            }

            var flight = await _context.Flight.FindAsync(id);

            if (flight == null)
            {
                return NotFound();
            }

            return flight;
        }

        [HttpPost("assign-passenger")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(PassengerFlightDetailsDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignPassengerToFlight([FromBody] PassengerFlightAssignmentDto assignmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _flightService.AddPassengerToFlight(assignmentDto);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error occurred while assigning passenger to flight.", details = ex.Message });
            }
        }

        [HttpGet("{flightId}/passengers")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PassengerNameDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPassengers(int flightId)
        {
            try
            {
                var passengers = await _flightService.GetPassengersOnFlight(flightId);
                return Ok(passengers);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Issue accured while trying to get passenger list", details = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFlight(int id)
        {
            try
            {
                bool wasDeleted = await _flightService.DeleteFlight(id);

                if (!wasDeleted)
                {
                    return NotFound($"Flight with ID {id} not found.");
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

        [HttpGet("{flightId}/spaceship")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PassengerNameDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetSpaceship(int flightId)
        {
            try
            {
                var spaceship = await _flightService.GetSpaceshipOnFlight(flightId);
                return Ok(spaceship);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Issue accured while trying to get spaceship", details = ex.Message });
            }
        }


        [HttpPost("assign-spaceship")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SpaceshipFlightAssignmentDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AssignSpaceshipToFlight([FromBody] SpaceshipFlightAssignmentDto assignmentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _flightService.AddSpaceshipToFlight(assignmentDto);

                return StatusCode(StatusCodes.Status201Created, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error occurred while assigning spaceship to flight.", details = ex.Message });
            }
        }

        [HttpGet("{flightId}/analysis")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FlightAnalysisDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFlightAnalysis(int flightId)
        {
            try
            {
                var analysisResult = await _flightService.GetFlightAnalysis(flightId);
                return Ok(analysisResult);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error during flight analysis retrieval.", details = ex.Message });
            }
        }
    }
}