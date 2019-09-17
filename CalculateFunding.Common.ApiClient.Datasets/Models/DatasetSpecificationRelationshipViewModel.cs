using CalculateFunding.Common.Models;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class DatasetSpecificationRelationshipViewModel : Reference
    {
        public DatasetDefinitionViewModel Definition { get; set; }

        public string DatasetName { get; set; }

        public int? Version { get; set; }

        public string DatasetId { get; set; }

        public string RelationshipDescription { get; set; }

        public bool IsProviderData { get; set; }
    }
}