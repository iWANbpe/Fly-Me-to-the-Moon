using Fly_Me_to_the_Moon.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Fly_Me_to_the_Moon.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CheckController : ControllerBase
    {

        private readonly SpaceFlightContext _context;

        public CheckController(SpaceFlightContext context)
        {
            _context = context;
        }


        [HttpGet("db")]
        public async Task<IActionResult> CheckDatabaseConnection()
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("SELECT 1;");

                return Ok(new
                {
                    Status = "Success"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Status = "Error",
                    Details = ex.Message
                });
            }
        }
    }
}
