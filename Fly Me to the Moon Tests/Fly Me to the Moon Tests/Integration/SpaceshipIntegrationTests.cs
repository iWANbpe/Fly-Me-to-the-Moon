using Fly_Me_to_the_Moon.Data;
using Fly_Me_to_the_Moon.Dtos;
using Fly_Me_to_the_Moon.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Fly_Me_to_the_Moon.Tests.Integration
{
    public class SpaceshipIntegrationTests : IClassFixture<ApiWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly ApiWebApplicationFactory _factory;
        private const string BaseUrl = "/api/Spaceship";

        public SpaceshipIntegrationTests(ApiWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task AddSpaceship_ShouldReturnCreated_WhenDataIsValid()
        {
            var uniqueName = $"Falcon-{Guid.NewGuid()}";
            var dto = new SpaceshipCreationDto
            {
                SpaceshipName = uniqueName,
                DateOfManufacture = DateTime.UtcNow.AddYears(-1),
                PassengerCapacity = 100,
                ContainersCapacity = 50
            };

            var response = await _client.PostAsJsonAsync(BaseUrl, dto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<SpaceshipDetailsDto>();
            Assert.NotNull(result);
            Assert.Equal(uniqueName, result.SpaceshipName);
            Assert.Equal(100, result.PassengerCapacity);
        }

        [Fact]
        public async Task AddSpaceship_ShouldReturnConflict_WhenSpaceshipAlreadyExists()
        {
            const string existingName = "Existing-Ship-01";

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<SpaceFlightContext>();

                if (!db.Spaceship.Any(s => s.SpaceshipName == existingName))
                {
                    db.Spaceship.Add(new Spaceship
                    {
                        SpaceshipName = existingName,
                        DateOfManufacture = DateTime.UtcNow,
                        PassengerCapacity = 10,
                        ContainersCapacity = 5
                    });
                    await db.SaveChangesAsync();
                }
            }

            var dto = new SpaceshipCreationDto
            {
                SpaceshipName = existingName,
                DateOfManufacture = DateTime.UtcNow,
                PassengerCapacity = 200,
                ContainersCapacity = 100
            };

            var response = await _client.PostAsJsonAsync(BaseUrl, dto);

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

            var errorBody = await response.Content.ReadAsStringAsync();
            Assert.Contains($"Spaceship with name '{existingName}' already exists", errorBody);
        }

        [Fact]
        public async Task AddSpaceship_ShouldReturnBadRequest_WhenNameIsEmpty()
        {
            var invalidDto = new SpaceshipCreationDto
            {
                SpaceshipName = "",
                DateOfManufacture = DateTime.UtcNow,
                PassengerCapacity = 10,
                ContainersCapacity = 5
            };

            var response = await _client.PostAsJsonAsync(BaseUrl, invalidDto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}