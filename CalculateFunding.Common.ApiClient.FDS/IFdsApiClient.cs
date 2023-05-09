using CalculateFunding.Common.ApiClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient.FDS
{
    public interface IFDSApiClient
    {
        Task<ApiResponse<dynamic>> GetFundingStream();
    }
}
