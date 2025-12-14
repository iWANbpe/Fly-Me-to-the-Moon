using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fly_Me_to_the_Moon.Models
{
    public class Passenger
    {
        [Key]
        public int PassengerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int AnalysisId { get; set; }
        public int InsuranceId { get; set; }

        public FullHealthAnalysisResult FullHealthAnalysisResult { get; set; } = null!;
        public Insurance Insurance { get; set; } = null!;
        public ICollection<PassengerFlight> PassengerFlight { get; set; } = new List<PassengerFlight>();
        public Baggage? Baggage { get; set; }
    }
}