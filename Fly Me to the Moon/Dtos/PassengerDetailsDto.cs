namespace Fly_Me_to_the_Moon.Dtos
{
    public class PassengerDetailsDto
    {
        public int PassengerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime? InsuranceExpireBy { get; set; }
    }
}
