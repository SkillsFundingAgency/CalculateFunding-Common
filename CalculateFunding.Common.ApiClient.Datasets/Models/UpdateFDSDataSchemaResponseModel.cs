using Newtonsoft.Json;
using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class UpdateFDSDataSchemaResponseModel
    {
        [JsonProperty("datasetDefinitionId")]
        public string DatasetDefintionId { get; set; }

        [JsonProperty("current")]
        public DefinitionSpecificationRelationshipVersion Current { get; set; }

        [JsonProperty("affectedCalculations")]
        public IEnumerable<AffectedCalculationResponseModel> AffectedCalculations { get; set; }
    }
}
