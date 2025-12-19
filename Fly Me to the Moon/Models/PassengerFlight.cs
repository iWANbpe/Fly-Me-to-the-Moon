using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Models
{
    public class PassengerFlight
    {
        public int? PassengerId { get; set; }
        public int? FlightId { get; set; }

        public Passenger? Passenger { get; set; }
        public Flight? Flight { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}