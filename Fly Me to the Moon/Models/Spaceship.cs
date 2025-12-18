using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Models
{
    public class Spaceship
    {
        [Key]
        public string SpaceshipName { get; set; } = string.Empty;

        public DateTime DateOfManufacture { get; set; }
        public int PassengerCapacity { get; set; }
        public int ContainersCapacity { get; set; }

        public ICollection<ServiceLog>? ServiceLog { get; set; }
        public ICollection<Flight>? Flight { get; set; }
    }
}