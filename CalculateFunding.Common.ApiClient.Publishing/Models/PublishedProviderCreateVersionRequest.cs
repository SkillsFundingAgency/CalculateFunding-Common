namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class PublishedProviderCreateVersionRequest
    {
        public PublishedProvider PublishedProvider { get; set; }

        public PublishedProviderVersion NewVersion { get; set; }
    }
}
