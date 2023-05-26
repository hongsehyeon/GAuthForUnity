using System.Threading.Tasks;

namespace GAuthForUnity
{
    public interface GAuth
    {
        Task<GAuthToken> GenerateTokenAsync(string email, string password, string clientId, string clientSecret, string redirectUri);

        Task<GAuthToken> GenerateTokenAsync(string code, string clientId, string clientSecret, string redirectUri);

        Task<GAuthCode> GenerateCodeAsync(string email, string password);

        Task<GAuthToken> RefreshAsync(string refreshToken);

        Task<GAuthUserInfo> GetUserInfoAsync(string accessToken);
    }
}