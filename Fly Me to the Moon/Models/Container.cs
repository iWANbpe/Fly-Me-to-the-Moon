using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Fly_Me_to_the_Moon.Models
{
    public class Container
    {
        [Key]
        public int ContainerId { get; set; }
        public int MaxWeight { get; set; }

        public ICollection<Baggage> Baggage { get; set; } = new List<Baggage>();
        public ICollection<ContainerFlight> ContainerFlight { get; set; } = new List<ContainerFlight>();
        public ICollection<Robot> Robot { get; set; } = new List<Robot>();

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}