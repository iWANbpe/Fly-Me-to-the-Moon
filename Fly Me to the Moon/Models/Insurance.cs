using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Models
{
    public class Insurance
    {
        [Key]
        public int InsuranceId { get; set; }
        public DateTime ExpireBy { get; set; }
        public string CompanyGrantedBy { get; set; } = string.Empty;

        public Passenger? Passenger { get; set; }
    }
}