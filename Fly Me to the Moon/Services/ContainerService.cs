using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Services
{
    public class ContainerService
    {
        private readonly SpaceFlightContext _context;

        public ContainerService(SpaceFlightContext context)
        {
            _context = context;
        }

        public async Task<Container> AddContainerToFlightAsync(ContainerCreationDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            Container container = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var flight = await _context.Flight
                        .Include(f => f.Spaceship)
                        .FirstOrDefaultAsync(f => f.FlightId == dto.FlightId);

                    if (flight == null)
                    {
                        throw new KeyNotFoundException($"Flight with ID {dto.FlightId} not found.");
                    }

                    if (flight.Spaceship == null)
                    {
                        throw new InvalidOperationException($"No spaceship assigned to flight {dto.FlightId}.");
                    }

                    var currentContainersCount = await _context.ContainerFlight
                        .CountAsync(cf => cf.FlightId == dto.FlightId);

                    if (currentContainersCount >= flight.Spaceship.ContainersCapacity)
                    {
                        throw new InvalidOperationException($"Capacity reached. Spaceship '{flight.Spaceship.SpaceshipName}' has a limit of {flight.Spaceship.ContainersCapacity} containers.");
                    }

                    container = new Container
                    {
                        MaxWeight = dto.MaxWeight,
                        RowVersion = 1
                    };

                    _context.Container.Add(container);
                    await _context.SaveChangesAsync();

                    var containerFlight = new ContainerFlight
                    {
                        ContainerId = container.ContainerId,
                        FlightId = dto.FlightId
                    };

                    _context.ContainerFlight.Add(containerFlight);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    if (ex is InvalidOperationException || ex is KeyNotFoundException) throw;
                    Console.WriteLine($"FATAL TRANSACTION ERROR: {ex.Message}");
                    throw;
                }
            });

            if (container == null) throw new ApplicationException("Container creation failed.");

            return container;
        }

        public async Task<Container> GetContainerByIdAsync(int id)
        {
            var container = await _context.Container
                .FirstOrDefaultAsync(c => c.ContainerId == id);

            if (container == null)
            {
                throw new KeyNotFoundException($"Container with ID {id} not found.");
            }

            return container;
        }

        public async Task<bool> DeleteContainerAsync(int id)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            bool deletionSuccessful = false;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var container = await _context.Container.FirstOrDefaultAsync(c => c.ContainerId == id);

                    if (container == null)
                    {
                        throw new KeyNotFoundException($"Container with ID {id} not found.");
                    }

                    _context.Container.Remove(container);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    deletionSuccessful = true;
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
                    throw new ApplicationException($"Container deletion failed. Details: {ex.Message}", ex);
                }
            });

            return deletionSuccessful;
        }
    }
}