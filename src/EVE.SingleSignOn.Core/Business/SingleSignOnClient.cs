using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EVE.SingleSignOn.Core
{
    public class SingleSignOnClient : ISingleSignOnClient
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly string _userAgent = "EVE.SingleSignOn.Core";

        public SingleSignOnClient(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Tokens> AuthorizeAsync(Uri uri, string clientId, string clientSecret, string code)
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code }
            };

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(body)
            };

            // Set the necessary headers
            request.Headers.Add("Authorization", $"Basic {BasicAuthorization(clientId, clientSecret)}");
            request.Headers.Add("Host", uri.Host);
            request.Headers.Add("User-Agent", _userAgent);

            return await Submit<Tokens>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="clientId"></param>
        /// <param name="codeVerifier"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<Tokens> AuthorizePKCEAsync(Uri uri, string clientId, string codeVerifier, string code)
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "client_id", clientId },
                { "code_verifier", codeVerifier }
            };

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(body)
            };

            // Set the necessary headers
            request.Headers.Add("Host", uri.Host);
            request.Headers.Add("User-Agent", _userAgent);

            return await Submit<Tokens>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="callbackUri"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public string GetAuthenticationUrl(Uri uri, string clientId, string scope, string callbackUri, string state)
        {
            string query = $"response_type=code&redirect_uri={Uri.EscapeDataString(callbackUri)}&client_id={Uri.EscapeDataString(clientId)}";

            if (!string.IsNullOrEmpty(scope))
            {
                query += $"&scope={Uri.EscapeDataString(scope)}";
            }

            if (!string.IsNullOrEmpty(state))
            {
                query += $"&state={Uri.EscapeDataString(state)}";
            }

            var builder = new UriBuilder(uri)
            {
                Query = query
            };

            return builder.Uri.AbsoluteUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <param name="callbackUri"></param>
        /// <param name="state"></param>
        /// <param name="codeChallenge"></param>
        /// <param name="challengeMethod"></param>
        /// <returns></returns>
        public string GetAuthenticationUrl(Uri uri, string clientId, string scope, string callbackUri, string state, string codeChallenge, string challengeMethod = "S256")
        {
            string query = $"response_type=code&redirect_uri={Uri.EscapeDataString(callbackUri)}&client_id={Uri.EscapeDataString(clientId)}&code_challenge={Uri.EscapeDataString(codeChallenge)}&code_challenge_method={Uri.EscapeDataString(challengeMethod)}";

            if (!string.IsNullOrEmpty(scope))
            {
                query += $"&scope={Uri.EscapeDataString(scope)}";
            }

            if (!string.IsNullOrEmpty(state))
            {
                query += $"&state={Uri.EscapeDataString(state)}";
            }

            var builder = new UriBuilder(uri)
            {
                Query = query
            };

            return builder.Uri.AbsoluteUri;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Tokens> RefreshAsync(Uri uri, string clientId, string clientSecret, string refreshToken)
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
            };

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(body)
            };

            // Set the necessary headers
            request.Headers.Add("Authorization", $"Basic {BasicAuthorization(clientId, clientSecret)}");
            request.Headers.Add("Host", uri.Host);
            request.Headers.Add("User-Agent", _userAgent);

            return await Submit<Tokens>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="clientId"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<Tokens> RefreshAsync(Uri uri, string clientId, string refreshToken)
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken },
                { "client_id", clientId }
            };

            var request = new HttpRequestMessage()
            {
                RequestUri = uri,
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(body)
            };

            // Set the necessary headers
            request.Headers.Add("Host", uri.Host);
            request.Headers.Add("User-Agent", _userAgent);

            return await Submit<Tokens>(request).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<T> Submit<T>(HttpRequestMessage request)
        {
            HttpResponseMessage response = await _httpClientFactory.CreateClient().SendAsync(request).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
        }

        /// <summary>
        /// Generate the base64 encoded string from clientId & clientSecret
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        private string BasicAuthorization(string clientId, string clientSecret)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
        }
    }
}
