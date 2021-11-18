using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public class PopulateCalculationResultQADatabaseRequest
    {
        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }
    }
}
