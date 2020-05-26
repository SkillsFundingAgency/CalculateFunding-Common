using CalculateFunding.Common.Models;
using CalculateFunding.Common.Utility;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CalculateFunding.Common.ApiClient
{
    public class UserProfilerPropagationMessageHandler : DelegatingHandler
    {
        private readonly IUserProfileProvider _userProfileProvider;

        public UserProfilerPropagationMessageHandler(IUserProfileProvider userProfileProvider)
        {
            Guard.ArgumentNotNull(userProfileProvider, nameof(userProfileProvider));
            _userProfileProvider = userProfileProvider;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            UserProfile userProfile = _userProfileProvider.UserProfile ?? new UserProfile("unknown", "unknown");

            bool hasContent = request.Content != null;

            if (!request.Headers.TryAddWithoutValidation(ApiClientHeaders.UserId, userProfile.Id) && hasContent)
            {
                request.Content.Headers.TryAddWithoutValidation(ApiClientHeaders.UserId, userProfile.Id);
            }

            if (!request.Headers.TryAddWithoutValidation(ApiClientHeaders.Username, userProfile.Name) && hasContent)
            {
                request.Content.Headers.TryAddWithoutValidation(ApiClientHeaders.Username, userProfile.Name);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
