using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Dtos
{
    public class BaggageRegistrationDto
    {
        [Required]
        public int PassengerId { get; set; }

        [Required]
        public int FlightId { get; set; }

        [Required]
        public double Weight { get; set; }

        [Required]
        public string Type { get; set; }
    }
}
