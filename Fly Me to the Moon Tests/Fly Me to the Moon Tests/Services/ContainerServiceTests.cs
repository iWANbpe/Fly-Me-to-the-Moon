using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Fly_Me_to_the_Moon.Services;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Tests.Services
{
    public class ContainerServiceTests
    {
        private SpaceFlightContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<SpaceFlightContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .ConfigureWarnings(x => x.Default(WarningBehavior.Ignore))
                .Options;
            return new SpaceFlightContext(options);
        }

        [Fact]
        public async Task AddContainer_ShouldThrowException_WhenSpaceshipCapacityIsFull()
        {
            using var context = GetDatabaseContext();

            var spaceship = new Spaceship
            {
                SpaceshipName = "TestShip",
                ContainersCapacity = 1
            };

            var flight = new Flight
            {
                FlightId = 10,
                SpaceshipName = "TestShip",
                Spaceship = spaceship
            };

            context.Spaceship.Add(spaceship);
            context.Flight.Add(flight);
            context.ContainerFlight.Add(new ContainerFlight { ContainerId = 1, FlightId = 10 });
            await context.SaveChangesAsync();

            var service = new ContainerService(context);
            var newContainerDto = new ContainerCreationDto { FlightId = 10, MaxWeight = 200 };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.AddContainerToFlightAsync(newContainerDto)
            );

            Assert.Contains("Capacity reached", exception.Message);
        }
    }
}