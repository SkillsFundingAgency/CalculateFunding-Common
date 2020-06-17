using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.External.Models
{
    public class PublishedFundingTemplate
    {
        public string MajorVersion { get; set; }
        public string MinorVersion { get; set; }
        public string PublishNote { get; set; }
        public string AuthorName { get; set; }
        public DateTime PublishDate { get; set; }
        public string SchemaVersion { get; set; }
    }
}
