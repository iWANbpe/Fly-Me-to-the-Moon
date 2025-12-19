using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Dtos
{
    public class SpaceshipFlightAssignmentDto
    {
        [Required]
        public int FlightId { get; set; }

        [Required]
        public string SpaceshipName { get; set; }
    }

}