using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Dtos
{
    public partial class FlightCreationDto
    {
        public class PassengerFlightAssignmentDto
        {
            [Required]
            public int FlightId { get; set; }

            [Required]
            public int PassengerId { get; set; }
        }
    }
}