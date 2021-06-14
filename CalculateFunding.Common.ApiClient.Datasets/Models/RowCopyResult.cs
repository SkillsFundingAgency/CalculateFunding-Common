namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class RowCopyResult
    {
        public ProviderConverter EligibleConverter { get; set; }

        public RowCopyOutcome Outcome { get; set; }

        public string ValidationMessage { get; set; }
    }
}