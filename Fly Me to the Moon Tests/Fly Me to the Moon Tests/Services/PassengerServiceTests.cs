using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Fly_Me_to_the_Moon.Services;
using Microsoft.EntityFrameworkCore;

namespace Fly_Me_to_the_Moon.Tests.Services
{
    public class PassengerServiceTests
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
        public async Task CreatePassengerWithDetails_ShouldSavePassenger_WhenDtoIsValid()
        {
            using var context = GetDatabaseContext();
            var service = new PassengerService(context);
            var dto = new PassengerRegistryRequestDto
            {
                Name = "Crazy Bee",
                PhoneNumber = "+380 67 555 9555",
                Email = "test@test.com"
            };

            var result = await service.CreatePassengerWithDetails(dto);

            Assert.NotNull(result);
            Assert.Equal("Crazy Bee", result.Name);
            Assert.Equal(1, await context.Passenger.CountAsync());
        }

        [Fact]
        public async Task DeletePassengerWithDetails_ShouldRemovePassengerAndLinkedData()
        {
            using var context = GetDatabaseContext();
            var service = new PassengerService(context);

            var insurance = new Insurance { InsuranceId = 1, ExpireBy = DateTime.Now.AddDays(10), RowVersion = 1 };
            var health = new FullHealthAnalysisResult { AnalysisId = 1, ExpireBy = DateTime.Now.AddDays(10), AllowedToFly = true, RowVersion = 1 };
            var passenger = new Passenger
            {
                PassengerId = 1,
                Name = "Bye bye poroh",
                Insurance = insurance,
                FullHealthAnalysisResult = health
            };

            context.Passenger.Add(passenger);
            await context.SaveChangesAsync();

            var result = await service.DeletePassengerWithDetails(1);

            Assert.True(result);
            Assert.Null(await context.Passenger.FindAsync(1));
            Assert.Null(await context.Insurance.FindAsync(1));
            Assert.Null(await context.FullHealthAnalysisResult.FindAsync(1));
        }

        [Fact]
        public async Task AddInsuranceAsync_ShouldLinkInsuranceToPassenger()
        {
            using var context = GetDatabaseContext();
            var service = new PassengerService(context);
            context.Passenger.Add(new Passenger { PassengerId = 1, Name = "Hetman", RowVersion = 1 });
            await context.SaveChangesAsync();

            var dto = new InsuranceDto { ExpireBy = DateTime.Now.AddDays(30), CompanyGrantedBy = "SpaceSafe" };

            var result = await service.AddInsuranceAsync(1, dto);

            Assert.NotNull(result.InsuranceId);
            var savedInsurance = await context.Insurance.FirstOrDefaultAsync(i => i.InsuranceId == result.InsuranceId);
            Assert.Equal("SpaceSafe", savedInsurance.CompanyGrantedBy);
        }

        [Fact]
        public async Task UpdatePassengerAndLinked_ShouldHandleConcurrencyAndIncrementVersion()
        {
            using var context = GetDatabaseContext();
            var service = new PassengerService(context);

            var passenger = new Passenger { PassengerId = 1, Name = "Old Name", RowVersion = 1 };
            context.Passenger.Add(passenger);
            await context.SaveChangesAsync();

            var updateDto = new UpdatePassengerDto
            {
                Name = "New Name",
                PhoneNumber = "123",
                Email = "new@test.com",
                PassengerRowVersion = 1
            };

            var result = await service.UpdatePassengerAndLinked(1, updateDto);

            Assert.Equal("New Name", result.Name);
            Assert.Equal(2, result.RowVersion);
        }
    }
}