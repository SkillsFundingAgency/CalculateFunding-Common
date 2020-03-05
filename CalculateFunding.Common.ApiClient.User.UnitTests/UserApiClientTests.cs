using System.Linq;
using System.Threading.Tasks;
using CalculateFunding.Common.ApiClient.Users;
using CalculateFunding.Common.ApiClient.Users.Models;
using CalculateFunding.Common.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog.Core;
using ApiUser = CalculateFunding.Common.ApiClient.Users.Models.User;
// ReSharper disable HeapView.CanAvoidClosure

namespace CalculateFunding.Common.ApiClient.User.UnitTests
{
    [TestClass]
    public class UserApiClientTests : ApiClientTestBase
    {
        private UsersApiClient _client;

        [TestInitialize]
        public void SetUp()
        {
            _client = new UsersApiClient(ClientFactory,
                Logger.None, default);
        }
        
        [TestMethod]
        public async Task GetUserByUserId()
        {
            string id = NewRandomString();

            await AssertGetRequest($"get-user-by-userid?userId={id}",
                new ApiUser(),
                () => _client.GetUserByUserId(id));
        }
        
        [TestMethod]
        public async Task ConfirmSkills()
        {
            string id = NewRandomString();
            UserConfirmModel model = new UserConfirmModel();
            
            await AssertPostRequest($"confirm-skills?userId={id}",
                model,
                new ApiUser(),
                () => _client.ConfirmSkills(id, model));
        }

        [TestMethod]
        public async Task GetFundingStreamPermissionsForUser()
        {
            string id = NewRandomString();

            await AssertGetRequest($"{id}/permissions",
                Enumerable.Empty<FundingStreamPermission>(),
                () => _client.GetFundingStreamPermissionsForUser(id));
        }

        [TestMethod]
        public async Task GetEffectivePermissionsForUser()
        {
            string userId = NewRandomString();
            string specificationId = NewRandomString();

            await AssertGetRequest($"{userId}/effectivepermissions/{specificationId}",
                new EffectiveSpecificationPermission(),
                () => _client.GetEffectivePermissionsForUser(userId, specificationId));
        }
        
        [TestMethod]
        public async Task UpdateFundingStreamPermission()
        {
            string userId = NewRandomString();
            string fundingStreamId = NewRandomString();
            FundingStreamPermissionUpdateModel model = new FundingStreamPermissionUpdateModel();

            await AssertPutRequest($"api/users/{userId}/permissions/{fundingStreamId}",
                model,
                new FundingStreamPermission(),
                () => _client.UpdateFundingStreamPermission(userId, fundingStreamId, model));
        }
    }
}