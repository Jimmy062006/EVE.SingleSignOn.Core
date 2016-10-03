using Newtonsoft.Json;

namespace EVE.SingleSignOn.Core
{
    public class SsoResponse
    {
        /// <summary>
        /// Access token to receive user details
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// Token type
        /// </summary>
        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        /// <summary>
        /// Expiration in seconds
        /// </summary>
        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        /// <summary>
        /// Refresh token, if any has been supplied
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
