using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Dtos
{
    public class FlightCreationDto
    {
        [Required]
        public string DeparturePoint { get; set; } = string.Empty;

        [Required]
        public DateTime DepartureDate { get; set; }

        [Required]
        public string PlaceOfArrival { get; set; } = string.Empty;

        [Required]
        public DateTime ArrivalDate { get; set; }

        public string? SpaceshipName { get; set; }
    }
}