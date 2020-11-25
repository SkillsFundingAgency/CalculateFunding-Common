using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Results.Models
{
    public class Job
    {
        [JsonProperty("id")]
        public string Id
        {
            get; set;
        }
    }
}