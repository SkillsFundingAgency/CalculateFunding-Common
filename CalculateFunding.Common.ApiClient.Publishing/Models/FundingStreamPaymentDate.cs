using System;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class FundingStreamPaymentDate
    {
        [JsonProperty("year")] 
        public int Year { get; set; }

        [JsonProperty("type")] 
        public ProfilePeriodType Type { get; set; }

        [JsonProperty("typeValue")] 
        public string TypeValue { get; set; }

        [JsonProperty("occurrence")] 
        public int Occurrence { get; set; }

        [JsonProperty("date")] 
        public DateTimeOffset Date { get; set; }
    }
}