using System.Threading.Tasks;

namespace EVE.SingleSignOn.Core
{
    public interface ISingleSignOnClient
    {
        /// <summary>
        /// Authenticating the code that we receive from the SSO
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<SsoResponse> AuthenticateAsync(string code);

        /// <summary>
        /// Retrieve the authentication URL for the EVE Online SSO
        /// </summary>
        /// <returns></returns>
        string GetAuthenticationUrl();

        /// <summary>
        /// Use a refresh token to receive a new authentication response
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<SsoResponse> RefreshAsync(string refreshToken);

        /// <summary>
        /// Verify an access token, receiving a character object from the EVE Online SSO
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        Task<SsoCharacter> VerifyAsync(string accessToken);
    }
}
