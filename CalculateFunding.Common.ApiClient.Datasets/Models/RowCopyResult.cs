namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public class RowCopyResult
    {
        public EligibleConverter EligibleConverter { get; set; }

        public RowCopyOutcome Outcome { get; set; }

        public string ValidationMessage { get; set; }
    }
}