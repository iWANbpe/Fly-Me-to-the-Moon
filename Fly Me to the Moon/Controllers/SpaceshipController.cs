using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fly_Me_to_the_Moon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaceshipController : ControllerBase
    {
        private readonly SpaceshipService _spaceshipService;

        public SpaceshipController(SpaceshipService spaceshipService)
        {
            _spaceshipService = spaceshipService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(SpaceshipDetailsDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AddSpaceship([FromBody] SpaceshipCreationDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newSpaceship = await _spaceshipService.AddSpaceshipAsync(dto);

                return CreatedAtAction(
                    nameof(GetSpaceshipById),
                    new { name = newSpaceship.SpaceshipName },
                    newSpaceship);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error creating spaceship.", details = ex.Message });
            }
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<SpaceshipDetailsDto>))]
        public async Task<IActionResult> GetAllSpaceships()
        {
            var spaceships = await _spaceshipService.GetAllSpaceshipsAsync();
            return Ok(spaceships);
        }

        [HttpGet("{name}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(SpaceshipDetailsDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSpaceshipById(string name)
        {
            try
            {
                var spaceship = await _spaceshipService.GetSpaceshipByIdAsync(name);
                return Ok(spaceship);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error retrieving spaceship.", details = ex.Message });
            }
        }

        [HttpDelete("{name}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSpaceship(string name)
        {
            try
            {
                await _spaceshipService.DeleteSpaceshipAsync(name);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Error deleting spaceship.", details = ex.Message });
            }
        }
    }
}