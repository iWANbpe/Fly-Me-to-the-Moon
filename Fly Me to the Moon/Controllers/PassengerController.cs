using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PassengerController : ControllerBase
    {
        private readonly SpaceFlightContext _context;
 
        public PassengerController(SpaceFlightContext context)
        {
            _context = context;
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

            var passenger = await _context.Passenger.FindAsync(id);

            if (passenger == null)
            {
                return NotFound();
            }

            return passenger;
        }

        [HttpPost]
        public async Task<ActionResult<Passenger>> PostPassenger(Passenger passenger)
        {
            if (_context.Passenger == null)
            {

                return Problem("Entity set 'SpaceFlightContext.Passengers' is null.");
            }

            _context.Passenger.Add(passenger);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetPassenger), new { id = passenger.PassengerId }, passenger);
        }
    }
}
