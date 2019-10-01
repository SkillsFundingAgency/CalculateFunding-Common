using System.Collections.Generic;

namespace CalculateFunding.Common.ApiClient.Policies.Models
{
    public class FundingStreamComparer : IEqualityComparer<FundingStream>
    {
        public bool Equals(FundingStream x, FundingStream y)
        {
            if (y == null && x == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else if (x.Id == y.Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(FundingStream obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
