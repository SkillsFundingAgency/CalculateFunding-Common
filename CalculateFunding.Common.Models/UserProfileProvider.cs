using System.Threading;

namespace CalculateFunding.Common.Models
{
    public class UserProfileProvider : IUserProfileProvider
    {
        private readonly static AsyncLocal<UserProfile> _userProfile = new AsyncLocal<UserProfile>();

        public UserProfile UserProfile
        {
            get { return _userProfile.Value; }
            set { _userProfile.Value = value; }
        }
    }
}
