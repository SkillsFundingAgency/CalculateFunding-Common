using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
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