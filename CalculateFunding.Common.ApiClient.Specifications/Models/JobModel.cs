using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class JobModel
    {
        [JsonProperty("jobId")]
        public string JobId { get; set; }
    }
}