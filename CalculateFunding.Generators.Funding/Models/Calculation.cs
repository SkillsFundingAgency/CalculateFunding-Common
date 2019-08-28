using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CalculateFunding.Generators.Funding.Enums;
using Newtonsoft.Json;

namespace CalculateFunding.Generators.Funding.Models
{
    /// <summary>
    /// A calculation used to build up a funding line.
    /// </summary>
    public class Calculation
    {
        public Calculation()
        {
        }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }

        [EnumDataType(typeof(CalculationType))]
        [JsonProperty("type")]
        public CalculationType Type { get; set; }

        [JsonProperty("calculations")]
        public IEnumerable<Calculation> Calculations { get; set; }
    }
}