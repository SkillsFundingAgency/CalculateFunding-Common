using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Jobs.Models
{
    public class JobCreateResult
    {
        [JsonProperty("createRequest")]
        public JobCreateModel CreateRequest { get; set; }
        
        [JsonProperty("job")]
        public Job Job { get; set; }
        
        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("wasQueued")]
        public bool WasQueued { get; set; }

        [JsonIgnore]
        public bool WasCreated => Job != null;

        [JsonIgnore]
        public bool HasError => !WasCreated || !WasQueued;
    }
}