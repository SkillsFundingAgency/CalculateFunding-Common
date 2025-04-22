using Newtonsoft.Json;
using System;

namespace CalculateFunding.Common.ApiClient.Profiling.Models
{
    public abstract class ProfilingRequestModelBase
    {
        public string FundingStreamId { get; set; }
        
        public string FundingPeriodId { get; set; }
        
        public string FundingLineCode { get; set; }
        
        public string ProfilePatternKey { get; set; }
        
        public string ProviderType { get; set; }
        
        public string ProviderSubType { get; set; }

        public DateTimeOffset? DateOpened { get; set; }
    }
}