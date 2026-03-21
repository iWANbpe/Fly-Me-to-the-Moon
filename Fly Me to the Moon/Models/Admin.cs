namespace Fly_Me_to_the_Moon.Models
{
    public class Admin
    {
        public int AdminId { get; set; }
        public string AdminName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
