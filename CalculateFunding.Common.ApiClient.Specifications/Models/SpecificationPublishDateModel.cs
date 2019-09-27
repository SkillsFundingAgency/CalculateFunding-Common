using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Specifications.Models
{
    public class SpecificationPublishDateModel
    {
        public DateTimeOffset? ExternalPublicationDate { get; set; }

        public DateTimeOffset? EarliestPaymentAvailableDate { get; set; }
    }
}
