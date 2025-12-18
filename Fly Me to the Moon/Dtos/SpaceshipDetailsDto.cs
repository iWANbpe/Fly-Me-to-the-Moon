namespace Fly_Me_to_the_Moon.Dtos
{
    public class SpaceshipDetailsDto
    {
        public string SpaceshipName { get; set; } = string.Empty;
        public DateTime DateOfManufacture { get; set; }
        public int PassengerCapacity { get; set; }
        public int ContainersCapacity { get; set; }
    }
}