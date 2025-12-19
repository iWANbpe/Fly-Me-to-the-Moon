namespace Fly_Me_to_the_Moon.Dtos
{
    public class FlightAnalysisDto
    {
        public int FlightId { get; set; }
        public string DeparturePoint { get; set; } = string.Empty;
        public string PlaceOfArrival { get; set; } = string.Empty;
        public DateTime DepartureDate { get; set; }
        public DateTime ArrivalDate { get; set; }

        public string SpaceshipName { get; set; } = string.Empty;

        public int CurrentPassengerCount { get; set; }
        public int MaxPassengerCapacity { get; set; }
        public double PassengeroccupancyPercentage { get; set; }

        public int ContainersCapacity { get; set; }

    }
}
