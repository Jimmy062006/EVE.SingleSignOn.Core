using System;
using System.Threading.Tasks;

namespace EVE.SingleSignOn.Core
{
    public interface ISingleSignOnClient
    {
        /// <summary>
        /// Generate an authroziation url
        /// </summary>
        /// <param name="uri">SSO URI including path</param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="callbackUri"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        string GetAuthenticationUrl(Uri uri, string clientId, string scope, string callbackUri, string state);

        /// <summary>
        /// Generate an authorization URI for PKCE
        /// </summary>
        /// <param name="uri">SSO URI including path</param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="callbackUri"></param>
        /// <param name="challengeMethod">S256</param>
        /// <param name="codeChallenge">URL safe Base64 encoded string</param>
        /// <param name="state"></param>
        /// <returns></returns>
        string GetAuthenticationUrl(Uri uri, string clientId, string scope, string callbackUri, string state, string codeChallenge, string challengeMethod = "S256");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri">SSO URI including path</param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<Tokens> AuthorizeAsync(Uri uri, string clientId, string clientSecret, string code);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri">SSO URI including path</param>
        /// <param name="clientId"></param>
        /// <param name="codeVerifier"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<Tokens> AuthorizePKCEAsync(Uri uri, string clientId, string codeVerifier, string code);

        /// <summary>
        /// Use a refresh token to receive a new authentication response
        /// </summary>
        /// <param name="uri">SSO URI including path</param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<Tokens> RefreshAsync(Uri uri, string clientId, string clientSecret, string refreshToken);

        /// <summary>
        /// Use a refresh token to receive a new authentication response (PKCE)
        /// </summary>
        /// <param name="uri">SSO URI including path</param>
        /// <param name="clientId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        Task<Tokens> RefreshAsync(Uri uri, string clientId, string refreshToken);
    }
}
