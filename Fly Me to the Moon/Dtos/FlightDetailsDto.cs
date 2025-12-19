namespace Fly_Me_to_the_Moon.Dtos
{
    public class FlightDetailsDto
    {
        public int FlightId { get; set; }
        public string DeparturePoint { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public string PlaceOfArrival { get; set; } = string.Empty;
        public DateTime ArrivalDate { get; set; }
        public string? SpaceshipName { get; set; }
    }
}