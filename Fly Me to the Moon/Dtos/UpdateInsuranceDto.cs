namespace Fly_Me_to_the_Moon.Dtos
{
    public class UpdateInsuranceDto
    {
        public DateTime ExpireBy { get; set; }
        public string CompanyGrantedBy { get; set; }
        public uint InsuranceRowVersion { get; set; }
    }
}
