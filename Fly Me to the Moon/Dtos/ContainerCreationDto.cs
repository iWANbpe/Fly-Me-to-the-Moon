using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Dtos
{
    public class ContainerCreationDto
    {
        [Required]
        public int FlightId { get; set; }

        [Required]
        public int MaxWeight { get; set; }
    }
}
