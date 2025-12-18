namespace Fly_Me_to_the_Moon.Dtos
{
    public partial class FlightCreationDto
    {
        public class PassengerFlightDetailsDto
        {
            public int PassengerId { get; set; }
            public int FlightId { get; set; }
        }
    }
}