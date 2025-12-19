using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Models
{
    public class Baggage
    {
        [Key]
        public int BaggageId { get; set; }
        public int MaxWeight { get; set; }
        public int? PassengerId { get; set; }
        public int? ContainerId { get; set; }
        public string Type { get; set; } = string.Empty;

        public Container? Container { get; set; } = null!;
        public Passenger? Passenger { get; set; }

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}