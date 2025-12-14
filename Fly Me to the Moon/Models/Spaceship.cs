using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Fly_Me_to_the_Moon.Models
{
    public class Spaceship
    {
        [Key]
        public string SpaceshipName { get; set; } = string.Empty;

        public DateTime DateOfManufacture { get; set; }
        public int PassengerCapacity { get; set; }
        public int ContainersCapacity { get; set; }

        public ServiceLog? ServiceLog { get; set; }
        public ICollection<Flight> Flight { get; set; } = new List<Flight>();
    }
}