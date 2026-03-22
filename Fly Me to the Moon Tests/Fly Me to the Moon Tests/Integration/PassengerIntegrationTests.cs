using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Fly_Me_to_the_Moon.Tests.Integration
{
    public class PassengerIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ApiWebApplicationFactory _factory;

        public PassengerIntegrationTests(ApiWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetExpiredInsurance_ShouldReturnOk_WhenAdminCalls()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SpaceFlightContext>();

                context.Passenger.RemoveRange(context.Passenger);
                context.Insurance.RemoveRange(context.Insurance);
                await context.SaveChangesAsync();

                var testId = 777;

                var insurance = new Insurance
                {
                    InsuranceId = testId,
                    ExpireBy = DateTime.SpecifyKind(new DateTime(2010, 1, 1), DateTimeKind.Utc),
                    RowVersion = 1
                };

                var passenger = new Passenger
                {
                    PassengerId = testId,
                    Name = "Expired Joe",
                    Email = "joe@test.com",
                    PhoneNumber = "123",
                    Insurance = insurance
                };

                context.Insurance.Add(insurance);
                context.Passenger.Add(passenger);
                await context.SaveChangesAsync();
            }

            var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
            var url = $"/api/Passenger/search/expired-insurance?Date={today}&pageNumber=1&pageSize=15";

            var response = await _client.GetAsync(url);

            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RegisterPassenger_ShouldReturnCreated_WhenDataIsValid()
        {
            var requestDto = new PassengerRegistryRequestDto
            {
                Name = "Ivan Test",
                Email = "ivan@test.com",
                PhoneNumber = "+380000000000"
            };

            var response = await _client.PostAsJsonAsync("/api/Passenger", requestDto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task DeletePassenger_ShouldReturnNoContent_WhenPassengerExists()
        {
            int id;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SpaceFlightContext>();
                var p = new Passenger { Name = "To Delete", Email = "d@t.com", PhoneNumber = "1", RowVersion = 1 };
                db.Passenger.Add(p);
                await db.SaveChangesAsync();
                id = p.PassengerId;
                db.ChangeTracker.Clear();
            }

            var response = await _client.DeleteAsync($"/api/Passenger/{id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task DeletePassenger_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            var response = await _client.DeleteAsync("/api/Passenger/9999");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}