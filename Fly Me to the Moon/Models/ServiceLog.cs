using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fly_Me_to_the_Moon.Models
{
    public class ServiceLog
    {
        [Key]
        public int LogId { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;

        public string SpaceshipName { get; set; } = string.Empty;
        public Spaceship Spaceship { get; set; } = null!;

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}