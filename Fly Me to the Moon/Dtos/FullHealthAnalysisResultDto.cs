namespace Fly_Me_to_the_Moon.Dtos
{
    public class FullHealthAnalysisResultDto
    {
        public DateTime ExpireBy { get; set; }
        public bool AllowedToFly { get; set; }
        public string GrantedBy { get; set; } = string.Empty;
    }
}
