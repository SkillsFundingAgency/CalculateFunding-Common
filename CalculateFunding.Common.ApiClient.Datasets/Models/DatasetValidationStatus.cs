namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public enum DatasetValidationStatus
    {
        Queued,
        Processing,
        ValidatingExcelWorkbook,
        ValidatingTableResults,
        SavingResults,
        Validated,
        FailedValidation,
        ExceptionThrown,
    }
}