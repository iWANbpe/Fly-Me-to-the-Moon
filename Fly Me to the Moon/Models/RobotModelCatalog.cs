using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Fly_Me_to_the_Moon.Models
{
    public class RobotModelCatalog
    {
        [Key]
        public string RobotModel { get; set; } = string.Empty;
        public int Weight { get; set; }

        public ICollection<Robot> Robot { get; set; } = new List<Robot>();

        [ConcurrencyCheck]
        public long RowVersion { get; set; }
    }
}