namespace CalculateFunding.Common.ApiClient.DataSets.Models
{
    public enum DatasetValidationStatus
    {
        Queued,
        Processing,
        ValidatingExcelWorkbook,
        MergeInprogress,
        MergeFailed,
        MergeCompleted,
        ValidatingTableResults,
        SavingResults,
        Validated,
        FailedValidation,
        ExceptionThrown,
    }
}