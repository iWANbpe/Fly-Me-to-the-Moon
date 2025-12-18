using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Dtos
{
    public class SpaceshipCreationDto
    {
        [Required]
        public string SpaceshipName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfManufacture { get; set; }

        [Required]
        public int PassengerCapacity { get; set; }

        [Required]
        public int ContainersCapacity { get; set; }
    }
}