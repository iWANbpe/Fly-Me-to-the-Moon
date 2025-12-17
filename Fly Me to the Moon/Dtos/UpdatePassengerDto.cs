namespace Fly_Me_to_the_Moon.Dtos
{
    public class UpdatePassengerDto
    {
        public int PassengerId { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public uint PassengerRowVersion { get; set; }

        public UpdateInsuranceDto InsuranceDetails { get; set; }
        public UpdateFullHealthAnalysisResultDto FHARDetails { get; set; }
    }
}
