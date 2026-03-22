using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Services;
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

        [HttpPost("register/admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterDto dto)
        {
            try
            {
                await _authService.RegisterAdmin(dto);
                return Ok(new { message = "Admin registered successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPost("register/passenger")]
        public async Task<IActionResult> RegisterPassenger([FromBody] RegisterDto dto)
        {
            try
            {
                await _authService.RegisterPassenger(dto);
                return Ok(new { message = "Passenger registered successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

    }
}
