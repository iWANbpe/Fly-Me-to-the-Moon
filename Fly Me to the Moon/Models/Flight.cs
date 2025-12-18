using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fly_Me_to_the_Moon.Models
{
    public class Flight
    {
        [Key]
        public int FlightId { get; set; }
        public string DeparturePoint { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public string PlaceOfArrival { get; set; } = string.Empty;
        public DateTime ArrivalDate { get; set; }

        public string? SpaceshipName { get; set; }
        public Spaceship? Spaceship { get; set; }
        public ICollection<ContainerFlight>? ContainerFlight { get; set; }
        public ICollection<PassengerFlight>? PassengerFlight { get; set; }
    }
}