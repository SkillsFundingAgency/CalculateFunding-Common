using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class QueueCalculationRunModel
    {
        public Reference Author { get; set; }

        public string CorrelationId { get; set; }

        public TriggerModel Trigger { get; set; }
    }
}
