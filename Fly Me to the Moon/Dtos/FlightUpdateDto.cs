namespace Fly_Me_to_the_Moon.Dtos
{
    public class FlightUpdateDto
    {
        public string? DeparturePoint { get; set; }
        public DateTime? DepartureDate { get; set; }
        public string? PlaceOfArrival { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public string? SpaceshipName { get; set; }
    }
}
