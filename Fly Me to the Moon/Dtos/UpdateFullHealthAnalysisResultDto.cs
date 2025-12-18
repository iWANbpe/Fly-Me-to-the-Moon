namespace Fly_Me_to_the_Moon.Dtos
{
    public class UpdateFullHealthAnalysisResultDto
    {
        public DateTime ExpireBy { get; set; }
        public bool AllowedToFly { get; set; }
        public string GrantedBy { get; set; }
        public long FHARRowVersion { get; set; }
    }
}
