using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Models
{
    public class Robot
    {
        [Key]
        public int RobotId { get; set; }
        public string RobotModel { get; set; } = string.Empty;
        public int ContainerId { get; set; }

        public RobotModelCatalog RobotModelCatalog { get; set; } = null!;
        public Container Container { get; set; } = null!;
    }
}