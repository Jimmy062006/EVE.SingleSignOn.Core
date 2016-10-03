using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EVE.SingleSignOn.Core
{
    public class SingleSignOnClient : ISingleSignOnClient
    {
        private readonly SsoSettings _settings;
        private readonly string _userAgent;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="settings"></param>
        public SingleSignOnClient(SsoSettings settings)
        {
            _settings = settings;
            _userAgent = "EVE.SingleSignOn.Core";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public async Task<SsoResponse> AuthenticateAsync(string code)
        {
            // Verify that we have settings
            if (_settings == null)
                throw new NullReferenceException("The SSO Client has not been initialized with settings.");

            // Verify that the code is not missing
            if (string.IsNullOrEmpty(code))
                throw new ArgumentNullException("Authentication code is null or empty");

            // Build the link to the SSO we will be using.
            var builder = new UriBuilder(_settings.BaseUrl)
            {
                Path = _settings.TokenPath,
                Query = $"grant_type=authorization_code&code={code}"
            };

            // Create a new request message
            var request = new HttpRequestMessage()
            {
                RequestUri = builder.Uri,
                Method = HttpMethod.Post
            };

            // Set the necessary headers
            request.Headers.Authorization = new AuthenticationHeaderValue($"{TokenType.Basic}", AuthorizationString());
            request.Headers.Add("Host", builder.Host);
            request.Headers.Add("User-Agent", _userAgent);
            
            return await CallSsoAsync<SsoResponse>(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetAuthenticationUrl()
        {
            // Verify that we have settings
            if (_settings == null)
                throw new NullReferenceException("The SSO Client has not been initialized with settings.");

            var builder = new UriBuilder(_settings.BaseUrl)
            {
                Path = _settings.AuthorizePath,
                Query = $"response_type=code&redirect_uri={_settings.CallbackUrl}&client_id={_settings.ClientId}&scope={_settings.Scope}&state={_settings.State}"
            };

            return builder.Uri.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public async Task<SsoResponse> RefreshAsync(string refreshToken)
        {
            // Verify that we have settings
            if (_settings == null)
                throw new NullReferenceException("The SSO Client has not been initialized with settings.");

            // Verify that the code is not missing
            if (string.IsNullOrEmpty(refreshToken))
                throw new ArgumentNullException("Authentication code is null or empty");

            // Build the link to the SSO we will be using.
            var builder = new UriBuilder(_settings.BaseUrl)
            {
                Path = _settings.TokenPath,
                Query = $"grant_type=refresh_token&refresh_token={refreshToken}"
            };

            // Create a new request message
            var request = new HttpRequestMessage()
            {
                RequestUri = builder.Uri,
                Method = HttpMethod.Post
            };

            // Set the necessary headers
            request.Headers.Authorization = new AuthenticationHeaderValue($"{TokenType.Basic}", AuthorizationString());
            request.Headers.Add("Host", builder.Host);
            request.Headers.Add("User-Agent", _userAgent);
            
            return await CallSsoAsync<SsoResponse>(request);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<SsoCharacter> VerifyAsync(string accessToken)
        {
            // Verify that we have settings
            if (_settings == null)
                throw new NullReferenceException("The SSO Client has not been initialized with settings.");

            // Verify that the code is not missing
            if (string.IsNullOrEmpty(accessToken))
                throw new ArgumentNullException("Authentication code is null or empty");

            // Build the link to the SSO we will be using.
            var builder = new UriBuilder(_settings.BaseUrl)
            {
                Path = _settings.VerifyPath
            };

            // Create a new request message
            var request = new HttpRequestMessage()
            {
                RequestUri = builder.Uri,
                Method = HttpMethod.Get
            };

            // Set the necessary headers
            request.Headers.Authorization = new AuthenticationHeaderValue($"{TokenType.Bearer}", accessToken);
            request.Headers.Add("Host", builder.Host);
            request.Headers.Add("User-Agent", _userAgent);
            
            return await CallSsoAsync<SsoCharacter>(request);
        }

        /// <summary>
        /// Authorization string takes the clientId and secret, and encodes it into a Base 64 string.
        /// When sent to the CCP Single Sign On service, it returns an access token, if both values are valid.
        /// </summary>
        /// <returns></returns>
        private string AuthorizationString()
        {
            if (_settings != null)
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(_settings.ClientId + ":" + _settings.ClientSecret));

            throw new NullReferenceException("The SSO Client has not been initialized with settings.");
        }

        /// <summary>
        /// Make the call to the configured SSO
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<T> CallSsoAsync<T>(HttpRequestMessage request)
        {
            T response = default(T);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                HttpResponseMessage resp = await client.SendAsync(request);

                // Check whether the SSO answered with 
                // a positive HTTP Status Code
                if (resp.IsSuccessStatusCode)
                {
                    // Deserialize the object into the response model
                    // which will be returned to the application
                    response = JsonConvert.DeserializeObject<T>(await resp.Content.ReadAsStringAsync());
                }
                else
                {
                    // Invalid code receieved, inform the user
                    throw new Exception("The SSO responded with a bad status code: " + resp.StatusCode);
                }
            }

            return response;
        }
    }
}
