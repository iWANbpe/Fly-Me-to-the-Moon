using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Fly_Me_to_the_Moon.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Fly_Me_to_the_Moon.Tests.Services
{
    public class SpaceshipServiceTests
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
        public async Task AddSpaceshipAsync_ShouldSuccess_WhenNameIsUnique()
        {
            using var context = GetDatabaseContext();
            var service = new SpaceshipService(context);

            var dto = new SpaceshipCreationDto
            {
                SpaceshipName = "Discovery One",
                DateOfManufacture = DateTime.Now.AddYears(-5),
                PassengerCapacity = 50,
                ContainersCapacity = 10
            };

            var result = await service.AddSpaceshipAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Discovery One", result.SpaceshipName);
            Assert.Equal(50, result.PassengerCapacity);

            var shipInDb = await context.Spaceship.FirstOrDefaultAsync(s => s.SpaceshipName == "Discovery One");
            Assert.NotNull(shipInDb);
        }

        [Fact]
        public async Task AddSpaceshipAsync_ShouldThrow_WhenNameAlreadyExists()
        {
            using var context = GetDatabaseContext();
            var service = new SpaceshipService(context);
            var duplicateName = "Millennium Falcon";

            context.Spaceship.Add(new Spaceship { SpaceshipName = duplicateName });
            await context.SaveChangesAsync();

            var dto = new SpaceshipCreationDto
            {
                SpaceshipName = duplicateName,
                DateOfManufacture = DateTime.Now,
                PassengerCapacity = 100,
                ContainersCapacity = 20
            };

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.AddSpaceshipAsync(dto));

            Assert.Contains(duplicateName, exception.Message);
        }

        [Fact]
        public async Task AddSpaceshipAsync_ShouldConvertDateToUtc()
        {
            using var context = GetDatabaseContext();
            var service = new SpaceshipService(context);
            var localDate = DateTime.SpecifyKind(new DateTime(2020, 1, 1), DateTimeKind.Local);

            var dto = new SpaceshipCreationDto
            {
                SpaceshipName = "Voyager",
                DateOfManufacture = localDate,
                PassengerCapacity = 10,
                ContainersCapacity = 5
            };

            await service.AddSpaceshipAsync(dto);
            var shipInDb = await context.Spaceship.FirstAsync(s => s.SpaceshipName == "Voyager");

            Assert.Equal(DateTimeKind.Utc, shipInDb.DateOfManufacture.Kind);
        }
    }
}