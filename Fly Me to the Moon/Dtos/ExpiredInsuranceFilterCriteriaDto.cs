namespace Fly_Me_to_the_Moon.Dtos
{
    public class ExpiredInsuranceFilterCriteriaDto
    {
        private DateTime _maxExpiryDate = DateTime.Today;

        public DateTime MaxExpiryDate
        {
            get => _maxExpiryDate;
            set
            {
                if (value.Kind == DateTimeKind.Unspecified)
                {
                    _maxExpiryDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                }
                else
                {
                    _maxExpiryDate = value.ToUniversalTime();
                }
            }
        }

        public string CompanyName { get; set; }
    }
}