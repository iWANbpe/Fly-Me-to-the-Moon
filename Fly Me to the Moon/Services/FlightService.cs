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

                        bool isOverlapping = await _context.Flight.AnyAsync(f => f.SpaceshipName == spaceship.SpaceshipName &&
                                                    ((departureDateUtc >= f.DepartureDate && departureDateUtc < f.ArrivalDate) ||
                                                     (arrivalDateUtc > f.DepartureDate && arrivalDateUtc <= f.ArrivalDate) ||
                                                     (departureDateUtc <= f.DepartureDate && arrivalDateUtc >= f.ArrivalDate)));

                        if (isOverlapping)
                        {
                            throw new InvalidOperationException($"The spaceship '{spaceship.SpaceshipName}' is already scheduled for another flight during this period.");
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

        public async Task<PassengerFlightDetailsDto> AddPassengerToFlight(PassengerFlightAssignmentDto dto)
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

                    var fhar = await _context.Set<FullHealthAnalysisResult>()
                        .FirstOrDefaultAsync(h => h.AnalysisId == passenger.AnalysisId);

                    if (fhar == null)
                    {
                        throw new InvalidOperationException($"Passenger ID {dto.PassengerId} has no completed health analysis. Cannot assign to flight.");
                    }

                    if (!fhar.AllowedToFly)
                    {
                        throw new InvalidOperationException($"Passenger ID {dto.PassengerId} is medically unfit to fly based on analysis");
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
                        deletionSuccessful = false;
                        return;
                    }

                    var passengerFlightsToDelete = await _context.PassengerFlight
                        .Where(pf => pf.FlightId == flightId)
                        .ToListAsync();

                    if (passengerFlightsToDelete.Any())
                    {
                        _context.PassengerFlight.RemoveRange(passengerFlightsToDelete);
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

        public async Task<SpaceshipFlightAssignmentDto> AddSpaceshipToFlight(SpaceshipFlightAssignmentDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            SpaceshipFlightAssignmentDto resultDto = null;

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

                    var spaceship = await _context.Spaceship.FirstOrDefaultAsync(s => s.SpaceshipName == dto.SpaceshipName);
                    if (spaceship == null)
                    {
                        throw new KeyNotFoundException($"Spaceship with name {dto.SpaceshipName} not found.");
                    }

                    var spaceshipAssignedFlight = await _context.Flight
                        .AnyAsync(f => f.SpaceshipName == dto.SpaceshipName && f.FlightId == dto.FlightId);

                    if (spaceshipAssignedFlight)
                    {
                        throw new InvalidOperationException($"{dto.SpaceshipName} spaceship is already assigned to the fligth with id {dto.FlightId}");
                    }

                    flight.SpaceshipName = dto.SpaceshipName;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

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

        public async Task<Spaceship> GetSpaceshipOnFlight(int flightId)
        {
            var flight = await _context.Flight.FirstOrDefaultAsync(f => f.FlightId == flightId);

            if (flight == null)
            {
                throw new KeyNotFoundException($"Flight with ID {flightId} not found.");
            }

            if (string.IsNullOrEmpty(flight.SpaceshipName))
            {
                throw new KeyNotFoundException($"Flight ID {flightId} does not have an assigned spaceship.");
            }

            var spaceship = await _context.Spaceship
                .Where(s => s.SpaceshipName == flight.SpaceshipName)
                .FirstOrDefaultAsync();

            return spaceship;
        }

        public async Task<FlightAnalysisDto> GetFlightAnalysis(int flightId)
        {
            var analysis = await _context.Flight
                .Where(f => f.FlightId == flightId)
                .Join(_context.Spaceship,
                      f => f.SpaceshipName,
                      s => s.SpaceshipName,
                      (f, s) => new { Flight = f, Spaceship = s })
                .Select(joined => new FlightAnalysisDto
                {
                    FlightId = joined.Flight.FlightId,
                    DeparturePoint = joined.Flight.DeparturePoint,
                    PlaceOfArrival = joined.Flight.PlaceOfArrival,
                    DepartureDate = joined.Flight.DepartureDate,
                    ArrivalDate = joined.Flight.ArrivalDate,

                    SpaceshipName = joined.Spaceship.SpaceshipName,

                    CurrentPassengerCount = _context.PassengerFlight.Count(pf => pf.FlightId == joined.Flight.FlightId),
                    MaxPassengerCapacity = joined.Spaceship.PassengerCapacity,
                    PassengeroccupancyPercentage = joined.Spaceship.PassengerCapacity == 0 ? 0 :
                        (double)_context.PassengerFlight.Count(pf => pf.FlightId == joined.Flight.FlightId) / joined.Spaceship.PassengerCapacity * 100,

                    ContainersCapacity = joined.Spaceship.ContainersCapacity
                })
                .FirstOrDefaultAsync();

            if (analysis == null)
            {
                throw new KeyNotFoundException($"Flight with ID {flightId} not found or has no assigned spaceship.");
            }

            return analysis;
        }

        public async Task<object> RegisterBaggageToFlight(BaggageRegistrationDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            object result = null!;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var isBooked = await _context.PassengerFlight
                        .AnyAsync(pf => pf.PassengerId == dto.PassengerId && pf.FlightId == dto.FlightId);

                    if (!isBooked)
                        throw new InvalidOperationException("Passenger is not assigned to this flight.");

                    double dynamicLimit = await GetMaxAllowedWeightPerPassenger(dto.FlightId);

                    if (dto.Weight > dynamicLimit)
                    {
                        throw new InvalidOperationException($"Baggage weight {dto.Weight}kg exceeds the personal dynamic limit of {dynamicLimit}kg.");
                    }

                    var containersWithLoads = await _context.ContainerFlight
                        .Where(cf => cf.FlightId == dto.FlightId)
                        .Select(cf => new
                        {
                            Container = cf.Container,
                            CurrentWeight = cf.Container.Baggage.Sum(b => b.MaxWeight)
                        })
                        .ToListAsync();

                    var target = containersWithLoads
                        .Where(c => c.CurrentWeight + dto.Weight <= c.Container.MaxWeight)
                        .OrderByDescending(c => c.CurrentWeight)
                        .FirstOrDefault();

                    if (target == null)
                        throw new InvalidOperationException("No space available in any container on this flight for this weight.");

                    var baggage = new Baggage
                    {
                        PassengerId = dto.PassengerId,
                        ContainerId = target.Container.ContainerId,
                        MaxWeight = dto.Weight,
                        Type = dto.Type,
                        RowVersion = 1
                    };

                    target.Container.RowVersion++;

                    _context.Baggage.Add(baggage);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    result = new
                    {
                        BaggageId = baggage.BaggageId,
                        ContainerId = baggage.ContainerId,
                        Weight = baggage.MaxWeight,
                        PersonalLimit = dynamicLimit,
                        Status = "Success"
                    };
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            return result;
        }

        public async Task<double> GetMaxAllowedWeightPerPassenger(int flightId)
        {
            var totalCapacity = await _context.ContainerFlight
                .Where(cf => cf.FlightId == flightId)
                .SumAsync(cf => (double)cf.Container.MaxWeight);

            var passengersCount = await _context.PassengerFlight
                .CountAsync(pf => pf.FlightId == flightId);

            if (passengersCount == 0) return totalCapacity;

            return Math.Floor(totalCapacity / passengersCount);
        }

        public async Task<Flight> UpdateFlight(int id, FlightUpdateDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            Flight flightToUpdate = null!;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    flightToUpdate = await _context.Flight
                        .Include(f => f.Spaceship)
                        .FirstOrDefaultAsync(f => f.FlightId == id);

                    if (flightToUpdate == null)
                        throw new KeyNotFoundException($"Flight with ID {id} not found.");

                    var departureUtc = dto.DepartureDate?.ToUniversalTime() ?? flightToUpdate.DepartureDate;
                    var arrivalUtc = dto.ArrivalDate?.ToUniversalTime() ?? flightToUpdate.ArrivalDate;

                    if (arrivalUtc <= departureUtc)
                        throw new InvalidOperationException("Arrival date cannot be earlier than or equal to departure date.");

                    string targetSpaceshipName = dto.SpaceshipName ?? flightToUpdate.SpaceshipName;

                    var spaceship = await _context.Spaceship
                        .FirstOrDefaultAsync(s => s.SpaceshipName == targetSpaceshipName);

                    if (spaceship == null)
                        throw new KeyNotFoundException($"Spaceship '{targetSpaceshipName}' not found.");

                    bool isBusy = await _context.Flight.AnyAsync(f =>
                        f.FlightId != id &&
                        f.SpaceshipName == spaceship.SpaceshipName &&
                        ((departureUtc >= f.DepartureDate && departureUtc < f.ArrivalDate) ||
                         (arrivalUtc > f.DepartureDate && arrivalUtc <= f.ArrivalDate) ||
                         (departureUtc <= f.DepartureDate && arrivalUtc >= f.ArrivalDate)));

                    if (isBusy)
                        throw new InvalidOperationException($"The spaceship '{spaceship.SpaceshipName}' is busy during this interval.");

                    if (dto.DeparturePoint != null) flightToUpdate.DeparturePoint = dto.DeparturePoint;
                    if (dto.PlaceOfArrival != null) flightToUpdate.PlaceOfArrival = dto.PlaceOfArrival;

                    flightToUpdate.DepartureDate = departureUtc;
                    flightToUpdate.ArrivalDate = arrivalUtc;
                    flightToUpdate.SpaceshipName = spaceship.SpaceshipName;
                    flightToUpdate.Spaceship = spaceship;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            return flightToUpdate;
        }
    }

}