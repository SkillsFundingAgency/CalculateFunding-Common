namespace CalculateFunding.Common.WebApi.Models
{
    public class DependencyHealth
    {
        public string DependencyName { get; set; }

        public bool HealthOk { get; set; }

        public string Message { get; set; }
    }
}
