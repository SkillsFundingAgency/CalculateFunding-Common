using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ProfileTotal
    {
        [JsonProperty("year")]
        public int Year { get; set; }
        
        [JsonProperty("typeValue")]
        public string TypeValue { get; set; }
        
        [JsonProperty("occurrence")]
        public int Occurrence { get; set; }
        
        [JsonProperty("value")]
        public decimal Value { get; set; }

        [JsonProperty("periodType")]
        public string PeriodType { get; set; }

        [JsonProperty("isPaid")]
        public bool IsPaid { get; set; }

        [JsonProperty("installmentNumber")]
        public int InstallmentNumber { get; set; }

        [JsonProperty("profileRemainingPercentage")]
        public decimal? ProfileRemainingPercentage { get; set; }

        [JsonProperty("actualDate")]
        public DateTimeOffset? ActualDate { get; set; }
    }
}