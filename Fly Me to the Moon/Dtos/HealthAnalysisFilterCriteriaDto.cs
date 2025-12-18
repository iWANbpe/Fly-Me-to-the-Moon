namespace Fly_Me_to_the_Moon.Dtos
{
    public class HealthAnalysisFilterCriteriaDto
    {

        private DateTime _expiryDate = DateTime.Today;

        public DateTime Date
        {
            get => _expiryDate;
            set
            {
                if (value.Kind == DateTimeKind.Unspecified)
                {
                    _expiryDate = DateTime.SpecifyKind(value, DateTimeKind.Utc);
                }
                else
                {
                    _expiryDate = value.ToUniversalTime();
                }
            }
        }
    }
}