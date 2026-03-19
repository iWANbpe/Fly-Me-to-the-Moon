using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Dtos
{
    public class AddPassengerDocumentsDto
    {
        [Required]
        public InsuranceDto Insurance { get; set; } = null!;

        [Required]
        public FullHealthAnalysisResultDto HealthAnalysis { get; set; } = null!;
    }
}
