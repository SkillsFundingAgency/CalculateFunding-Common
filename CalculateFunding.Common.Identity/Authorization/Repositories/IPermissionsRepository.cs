using System.Collections.Generic;
using System.Threading.Tasks;
using CalculateFunding.Common.Identity.Authorization.Models;

namespace CalculateFunding.Common.Identity.Authorization.Repositories
{
    public interface IPermissionsRepository
    {
        Task<EffectiveSpecificationPermission> GetPermissionForUserBySpecificationId(string userId, string specificationId);

        Task<IEnumerable<FundingStreamPermission>> GetPermissionsForUser(string userId);
    }
}
