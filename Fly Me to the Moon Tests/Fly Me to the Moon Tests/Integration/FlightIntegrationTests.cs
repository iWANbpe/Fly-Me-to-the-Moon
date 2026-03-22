using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Fly_Me_to_the_Moon.Tests.Integration
{
    public class FlightIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ApiWebApplicationFactory _factory;
        private const string BaseUrl = "/api/Flight/assign-passenger";

        public FlightIntegrationTests(ApiWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AssignPassengerToFlight_ShouldReturnConflict_WhenPassengerAlreadyAssigned()
        {
            int pId;
            int fId;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SpaceFlightContext>();

                db.PassengerFlight.RemoveRange(db.PassengerFlight);
                db.Passenger.RemoveRange(db.Passenger);
                db.Flight.RemoveRange(db.Flight);
                await db.SaveChangesAsync();

                var health = new FullHealthAnalysisResult { AllowedToFly = true };
                db.Set<FullHealthAnalysisResult>().Add(health);
                await db.SaveChangesAsync();

                var passenger = new Passenger { Name = "Ivan", Email = "ivan@test.com", PhoneNumber = "123", AnalysisId = health.AnalysisId, RowVersion = 1 };
                db.Passenger.Add(passenger);

                var flight = new Flight { DeparturePoint = "Earth", PlaceOfArrival = "Moon", DepartureDate = DateTime.UtcNow.AddDays(1), ArrivalDate = DateTime.UtcNow.AddDays(2), RowVersion = 1 };
                db.Flight.Add(flight);
                await db.SaveChangesAsync();

                pId = passenger.PassengerId;
                fId = flight.FlightId;

                db.PassengerFlight.Add(new PassengerFlight { FlightId = fId, PassengerId = pId });
                await db.SaveChangesAsync();
            }

            var dto = new PassengerFlightAssignmentDto { FlightId = fId, PassengerId = pId };
            var response = await _client.PostAsJsonAsync(BaseUrl, dto);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task AssignPassengerToFlight_ShouldReturnConflict_WhenPassengerHasNoHealthAnalysis()
        {
            int pId;
            int fId;

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SpaceFlightContext>();

                var passenger = new Passenger { Name = "NoHealth", Email = "no@health.com", PhoneNumber = "000", RowVersion = 1 };
                var flight = new Flight { DeparturePoint = "A", PlaceOfArrival = "B", DepartureDate = DateTime.UtcNow.AddDays(1), ArrivalDate = DateTime.UtcNow.AddDays(2), RowVersion = 1 };

                db.Passenger.Add(passenger);
                db.Flight.Add(flight);
                await db.SaveChangesAsync();

                pId = passenger.PassengerId;
                fId = flight.FlightId;
            }

            var dto = new PassengerFlightAssignmentDto { FlightId = fId, PassengerId = pId };
            var response = await _client.PostAsJsonAsync(BaseUrl, dto);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }
}