using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Services
{
    public class SpaceshipService
    {
        private readonly SpaceFlightContext _context;

        public SpaceshipService(SpaceFlightContext context)
        {
            _context = context;
        }

        public async Task<SpaceshipDetailsDto> AddSpaceshipAsync(SpaceshipCreationDto dto)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            Spaceship spaceship = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var existingSpaceship = await _context.Spaceship.FirstOrDefaultAsync(s => s.SpaceshipName == dto.SpaceshipName);
                    if (existingSpaceship != null)
                    {
                        throw new InvalidOperationException($"Spaceship with name '{dto.SpaceshipName}' already exists.");
                    }

                    spaceship = new Spaceship
                    {
                        SpaceshipName = dto.SpaceshipName,
                        DateOfManufacture = dto.DateOfManufacture.ToUniversalTime(),
                        PassengerCapacity = dto.PassengerCapacity,
                        ContainersCapacity = dto.ContainersCapacity
                    };

                    _context.Spaceship.Add(spaceship);
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

            if (spaceship == null) throw new ApplicationException("Spaceship creation failed.");

            return new SpaceshipDetailsDto
            {
                SpaceshipName = spaceship.SpaceshipName,
                DateOfManufacture = spaceship.DateOfManufacture,
                PassengerCapacity = spaceship.PassengerCapacity,
                ContainersCapacity = spaceship.ContainersCapacity
            };
        }

        public async Task<List<SpaceshipDetailsDto>> GetAllSpaceshipsAsync()
        {
            return await _context.Spaceship
                .Select(s => new SpaceshipDetailsDto
                {
                    SpaceshipName = s.SpaceshipName,
                    DateOfManufacture = s.DateOfManufacture,
                    PassengerCapacity = s.PassengerCapacity,
                    ContainersCapacity = s.ContainersCapacity
                })
                .ToListAsync();
        }

        public async Task<SpaceshipDetailsDto> GetSpaceshipByIdAsync(string name)
        {
            var spaceship = await _context.Spaceship
                .FirstOrDefaultAsync(s => s.SpaceshipName == name);

            if (spaceship == null)
            {
                throw new KeyNotFoundException($"Spaceship '{name}' not found.");
            }

            return new SpaceshipDetailsDto
            {
                SpaceshipName = spaceship.SpaceshipName,
                DateOfManufacture = spaceship.DateOfManufacture,
                PassengerCapacity = spaceship.PassengerCapacity,
                ContainersCapacity = spaceship.ContainersCapacity
            };
        }

        public async Task<bool> DeleteSpaceshipAsync(string name)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            bool deletionSuccessful = false;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    var spaceship = await _context.Spaceship.FirstOrDefaultAsync(s => s.SpaceshipName == name);

                    if (spaceship == null)
                    {
                        throw new KeyNotFoundException($"Spaceship '{name}' not found.");
                    }

                    _context.Spaceship.Remove(spaceship);
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
                    throw new ApplicationException($"Spaceship deletion failed. Transaction rolled back. Details: {ex.Message}", ex);
                }
            });

            return deletionSuccessful;
        }
    }
}