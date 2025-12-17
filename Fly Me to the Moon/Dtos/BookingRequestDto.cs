using System.ComponentModel.DataAnnotations;

namespace Fly_Me_to_the_Moon.Dtos
{
    public class BookingRequestDto
    {

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(32)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Insurance details are required.")]
        public InsuranceDto InsuranceDetails { get; set; } = null!;

        [Required(ErrorMessage = "Details of Full Health Analysis Result(FHAR) are required.")]
        public FullHealthAnalysisResultDto FullHealthAnalysisResultDetails { get; set; } = null!;

        public BaggageDto? BaggageDetails { get; set; }
    }
}