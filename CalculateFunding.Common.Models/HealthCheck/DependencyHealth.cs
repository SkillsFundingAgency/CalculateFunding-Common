namespace CalculateFunding.Common.Models.HealthCheck
{
    public class DependencyHealth
    {
        public string DependencyName { get; set; }

        public bool HealthOk { get; set; }

        public string Message { get; set; }
    }
}
