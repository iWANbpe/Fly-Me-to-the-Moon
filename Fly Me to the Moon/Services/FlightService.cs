using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Services
{
    public class FlightService
    {
        private readonly SpaceFlightContext _context;

        public FlightService(SpaceFlightContext context)
        {
            _context = context;
        }

        public async Task<Flight> AddFlight(FlightCreationDto flightDto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            Flight flight = null;

            await strategy.ExecuteAsync(async () =>
            {

                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    if (flightDto.ArrivalDate <= flightDto.DepartureDate)
                    {
                        throw new InvalidOperationException("Arrival date cannot be earlier than departure date");
                    }

                    var departureDateUtc = flightDto.DepartureDate.ToUniversalTime();
                    var arrivalDateUtc = flightDto.ArrivalDate.ToUniversalTime();

                    flight = new Flight
                    {
                        DeparturePoint = flightDto.DeparturePoint,
                        DepartureDate = departureDateUtc,
                        PlaceOfArrival = flightDto.PlaceOfArrival,
                        ArrivalDate = arrivalDateUtc,
                        SpaceshipName = flightDto.SpaceshipName,
                        PassengerFlight = new List<PassengerFlight>()
                    };

                    if (!string.IsNullOrEmpty(flightDto.SpaceshipName))
                    {
                        var spaceship = await _context.Spaceship
                            .FirstOrDefaultAsync(s => s.SpaceshipName == flightDto.SpaceshipName);

                        if (spaceship == null)
                        {
                            throw new KeyNotFoundException($"Couldn't find '{flightDto.SpaceshipName}' spaceship");
                        }
                        flight.Spaceship = spaceship;
                    }


                    _context.Flight.Add(flight);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (InvalidOperationException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (KeyNotFoundException)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"FATAL TRANSACTION ERROR: {ex.Message}");
                    throw new ApplicationException("Flight creation failed. Transaction rolled back.", ex);
                }
            });

            return flight;
        }
    }
}