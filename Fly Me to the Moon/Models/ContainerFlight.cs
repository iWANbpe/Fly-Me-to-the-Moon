using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Models
{
    public class ContainerFlight
    {
        public int ContainerId { get; set; }
        public int FlightId { get; set; }

        public Container Container { get; set; } = null!;
        public Flight Flight { get; set; } = null!;

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}