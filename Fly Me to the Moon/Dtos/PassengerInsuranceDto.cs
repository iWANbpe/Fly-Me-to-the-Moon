namespace Fly_Me_to_the_Moon.Dtos
{
    public class PassengerInsuranceDto
    {
        public int PassengerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public InsuranceDto Insurance { get; set; } = new InsuranceDto();
    }
}