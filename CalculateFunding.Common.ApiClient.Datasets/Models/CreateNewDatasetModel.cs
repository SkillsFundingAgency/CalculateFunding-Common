using System.ComponentModel.DataAnnotations;

namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class CreateNewDatasetModel
    {
        public string DefinitionId { get; set; }

        public string Filename { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FundingStreamId { get; set; }
        public bool ConverterWizard { get; set; }
    }
}