using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Microsoft.EntityFrameworkCore;
using static Fly_Me_to_the_Moon.Dtos.FlightCreationDto;

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
                        SpaceshipName = flightDto.SpaceshipName
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

        public async Task<PassengerFlightDetailsDto> AddPassengerToFlightAsync(PassengerFlightAssignmentDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            PassengerFlightDetailsDto resultDto = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {                 
                    var flight = await _context.Flight.FirstOrDefaultAsync(f => f.FlightId == dto.FlightId);
                    if (flight == null)
                    {
                        throw new KeyNotFoundException($"Flight with ID {dto.FlightId} not found.");
                    }

                    var passenger = await _context.Passenger.FirstOrDefaultAsync(p => p.PassengerId == dto.PassengerId);
                    if (passenger == null)
                    {
                        throw new KeyNotFoundException($"Passenger with ID {dto.PassengerId} not found.");
                    }

                    var alreadyAssigned = await _context.PassengerFlight
                        .AnyAsync(pf => pf.FlightId == dto.FlightId && pf.PassengerId == dto.PassengerId);

                    if (alreadyAssigned)
                    {
                        throw new InvalidOperationException($"Passenger ID {dto.PassengerId} is already assigned to Flight ID {dto.FlightId}.");
                    }

                    var passengerFlight = new PassengerFlight
                    {
                        FlightId = dto.FlightId,
                        PassengerId = dto.PassengerId,
                        Flight = flight,
                        Passenger = passenger
                    };

                    _context.PassengerFlight.Add(passengerFlight);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    resultDto = new PassengerFlightDetailsDto
                    {
                        FlightId = dto.FlightId,
                        PassengerId = dto.PassengerId
                    };
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"FATAL TRANSACTION ERROR: {ex.Message}");
                    throw;
                }
            });

            return resultDto;
        }

        public async Task<List<PassengerNameDto>> GetPassengersOnFlight(int flightId)
        {
            var flightExists = await _context.Flight.AnyAsync(f => f.FlightId == flightId);
            if (!flightExists)
            {
                throw new KeyNotFoundException($"Flight with ID {flightId} not found.");
            }

            var passengers = await _context.PassengerFlight
                .Where(pf => pf.FlightId == flightId)
                .Select(pf => pf.Passenger)
                .Select(p => new PassengerNameDto
                {
                    PassengerId = p.PassengerId,
                    Name = p.Name
                })
                .ToListAsync();

            return passengers;
        }
        public async Task<bool> DeleteFlight(int flightId)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            bool deletionSuccessful = false;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var flight = await _context.Flight.FirstOrDefaultAsync(f => f.FlightId == flightId);

                    if (flight == null)
                    {
                        await transaction.CommitAsync();
                        return;
                    }

                    _context.Flight.Remove(flight);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    deletionSuccessful = true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"FATAL DELETION ERROR: {ex.Message}");
                    throw new ApplicationException($"Flight deletion failed. Transaction rolled back. Details: {ex.Message}", ex);
                }
            });

            return deletionSuccessful;
        }
    }
}
