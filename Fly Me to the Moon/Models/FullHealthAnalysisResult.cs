using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fly_Me_to_the_Moon.Models
{
    public class FullHealthAnalysisResult
    {
        [Key]
        public int AnalysisId { get; set; }
        public DateTime ExpireBy { get; set; }
        public bool AllowedToFly { get; set; }
        public string GrantedBy { get; set; } = string.Empty;

        public Passenger? Passenger { get; set; }

        [Timestamp]
        public uint RowVersion { get; set; }
    }
}