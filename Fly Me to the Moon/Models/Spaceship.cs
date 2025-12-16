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

        public ICollection<ServiceLog> ServiceLog { get; set; } = new List<ServiceLog>();
        public ICollection<Flight> Flight { get; set; } = new List<Flight>();
    }
}