using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fly_Me_to_the_Moon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login/admin")]
        public async Task<IActionResult> AdminLogin([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginAdmin(dto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [HttpPost("login/passenger")]
        public async Task<IActionResult> PassengerLogin([FromBody] LoginDto dto)
        {
            try
            {
                var token = await _authService.LoginPassenger(dto);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
    }
}
