using CalculateFunding.Common.Models;
using Newtonsoft.Json;

namespace CalculateFunding.Common.ApiClient.Calcs.Models
{
    public class CompilerOptions : IIdentifiable
    {
        [JsonProperty("id")]
        public string Id => SpecificationId;

        [JsonProperty("specificationId")]
        public string SpecificationId { get; set; }

        /// <summary>
        /// Use VB.NET Option Strict
        /// </summary>
        [JsonProperty("optionStrictEnabled")]
        public bool OptionStrictEnabled { get; set; } = true;

        /// <summary>
        /// Generate legacy code functions and properties which come from The Store. eg LaToProv and IIf
        /// </summary>
        [JsonProperty("useLegacyCode")]
        public bool UseLegacyCode { get; set; }
    }
}
