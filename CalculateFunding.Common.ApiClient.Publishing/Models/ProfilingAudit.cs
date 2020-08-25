using CalculateFunding.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CalculateFunding.Common.ApiClient.Publishing.Models
{
    public class ProfilingAudit
    {
        public string FundingLineCode { get; set; }
        public Reference User { get; set; }
        public DateTime Date { get; set; }

        public override bool Equals(object obj) => obj?.GetHashCode().Equals(GetHashCode()) == true;

        public override int GetHashCode() => HashCode.Combine(FundingLineCode,
            User,
            Date);

        public override string ToString() => $"{FundingLineCode}:{User?.Name}:{Date:yyy-MM-ddTHH:mm:ss}";
    }
}
