using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Fly_Me_to_the_Moon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Fly_Me_to_the_Moon.Tests.Services
{
    public class FlightServiceTests
    {
        private SpaceFlightContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<SpaceFlightContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .ConfigureWarnings(w => w.Default(WarningBehavior.Ignore))
                .Options;
            return new SpaceFlightContext(options);
        }

        [Fact]
        public async Task AddFlight_ShouldThrowException_WhenArrivalBeforeDeparture()
        {
            using var context = GetDatabaseContext();
            var service = new FlightService(context);
            var dto = new FlightCreationDto
            {
                DepartureDate = DateTime.Now.AddDays(2),
                ArrivalDate = DateTime.Now.AddDays(1),
                DeparturePoint = "Earth",
                PlaceOfArrival = "Moon",
                SpaceshipName = "Apollo"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddFlight(dto));
        }

        [Fact]
        public async Task AddFlight_ShouldThrowException_WhenSpaceshipIsOverlapping()
        {
            using var context = GetDatabaseContext();
            var service = new FlightService(context);
            var shipName = "Nautilus";

            context.Spaceship.Add(new Spaceship { SpaceshipName = shipName });
            context.Flight.Add(new Flight
            {
                SpaceshipName = shipName,
                DepartureDate = DateTime.UtcNow.AddHours(10),
                ArrivalDate = DateTime.UtcNow.AddHours(20)
            });
            await context.SaveChangesAsync();

            var dto = new FlightCreationDto
            {
                SpaceshipName = shipName,
                DepartureDate = DateTime.UtcNow.AddHours(15),
                ArrivalDate = DateTime.UtcNow.AddHours(25)
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddFlight(dto));
        }

        [Fact]
        public async Task AddPassengerToFlight_ShouldThrowException_WhenFHARProhibits()
        {
            using var context = GetDatabaseContext();
            var service = new FlightService(context);

            var health = new FullHealthAnalysisResult { AnalysisId = 1, AllowedToFly = false };
            var passenger = new Passenger { PassengerId = 1, Name = "Iwan", AnalysisId = 1 };
            var flight = new Flight { FlightId = 10, DeparturePoint = "Moon" };

            context.Set<FullHealthAnalysisResult>().Add(health);
            context.Passenger.Add(passenger);
            context.Flight.Add(flight);
            await context.SaveChangesAsync();

            var dto = new PassengerFlightAssignmentDto { FlightId = 10, PassengerId = 1 };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.AddPassengerToFlight(dto));
            Assert.Contains("medically unfit", exception.Message);
        }

        [Fact]
        public async Task AddPassengerToFlight_ShouldSuccess_WhenDataIsValid()
        {
            using var context = GetDatabaseContext();
            var service = new FlightService(context);

            var health = new FullHealthAnalysisResult { AnalysisId = 7, AllowedToFly = true };
            var passenger = new Passenger { PassengerId = 7, Name = "Rey", AnalysisId = 7 };
            var flight = new Flight { FlightId = 77, DeparturePoint = "Moon" };

            context.Set<FullHealthAnalysisResult>().Add(health);
            context.Passenger.Add(passenger);
            context.Flight.Add(flight);
            await context.SaveChangesAsync();

            var dto = new PassengerFlightAssignmentDto { FlightId = 77, PassengerId = 7 };

            var result = await service.AddPassengerToFlight(dto);

            Assert.NotNull(result);
            Assert.Equal(77, result.FlightId);
            Assert.True(await context.PassengerFlight.AnyAsync(pf => pf.PassengerId == 7));
        }

        [Fact]
        public async Task RegisterBaggage_ShouldThrow_WhenPassengerNotAssignedToFlight()
        {
            using var context = GetDatabaseContext();
            var service = new FlightService(context);

            var dto = new BaggageRegistrationDto
            {
                PassengerId = 1,
                FlightId = 10,
                Weight = 10
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.RegisterBaggageToFlight(dto));
        }

        [Fact]
        public async Task RegisterBaggage_ShouldThrow_WhenNoSpaceInContainers()
        {
            using var context = GetDatabaseContext();
            var service = new FlightService(context);

            var container = new Container { ContainerId = 1, MaxWeight = 50 };
            context.Container.Add(container);
            context.PassengerFlight.Add(new PassengerFlight { PassengerId = 1, FlightId = 10 });
            context.ContainerFlight.Add(new ContainerFlight { ContainerId = 1, FlightId = 10 });

            context.Baggage.Add(new Baggage { ContainerId = 1, MaxWeight = 45, PassengerId = 2 });
            await context.SaveChangesAsync();

            var dto = new BaggageRegistrationDto
            {
                PassengerId = 1,
                FlightId = 10,
                Weight = 10
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.RegisterBaggageToFlight(dto));
            Assert.Contains("No space available", exception.Message);
        }

        [Fact]
        public async Task RegisterBaggage_ShouldSelectMostFilledContainerThatFits()
        {
            using var context = GetDatabaseContext();
            var service = new FlightService(context);

            context.PassengerFlight.Add(new PassengerFlight { PassengerId = 1, FlightId = 10 });

            context.Container.AddRange(
                new Container { ContainerId = 1, MaxWeight = 50 },
                new Container { ContainerId = 2, MaxWeight = 50 }
            );
            context.ContainerFlight.AddRange(
                new ContainerFlight { ContainerId = 1, FlightId = 10 },
                new ContainerFlight { ContainerId = 2, FlightId = 10 }
            );

            context.Baggage.Add(new Baggage { ContainerId = 1, MaxWeight = 40, PassengerId = 2 });
            await context.SaveChangesAsync();

            var dto = new BaggageRegistrationDto
            {
                PassengerId = 1,
                FlightId = 10,
                Weight = 5,
                Type = "Standard"
            };

            var result = await service.RegisterBaggageToFlight(dto);

            var containerId = result.GetType().GetProperty("ContainerId")?.GetValue(result, null);
            var status = result.GetType().GetProperty("Status")?.GetValue(result, null);

            Assert.Equal(1, containerId);
            Assert.Equal("Success", status);
        }
    }
}