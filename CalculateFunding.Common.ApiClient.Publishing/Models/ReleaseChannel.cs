namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ReleaseChannel
    {
        public string ChannelName { get; set; }
        public string ChannelCode { get; set; }
        public int MajorVersion { get; set; }
        public int MinorVersion { get; set; }
    }
}