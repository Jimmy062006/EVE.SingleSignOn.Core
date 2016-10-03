using System;

namespace EVE.SingleSignOn.Core
{
    public class SsoSettings
    {
        public readonly string TokenPath = "oauth/token";
        public readonly string AuthorizePath = "oauth/authorize";
        public readonly string VerifyPath = "oauth/verify";

        /// <summary>
        /// Base URL for the SSO you're connecting to.
        /// Default value should be https://login.eveonline.com/
        /// </summary>
        public string BaseUrl { get; set; }

        /// <summary>
        /// Callback URL for your application, you set this value in the EVE Online Developers console
        /// If you haven't made one yet, you can create one at https://developers.eveonline.com/
        /// </summary>
        public string CallbackUrl { get; set; }

        /// <summary>
        /// The requested scopes as a space delimited string
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// Client ID as provided by the EVE Online Developers console
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Client secret as provided by the EVE Online Developers console
        /// </summary>
        public string ClientSecret { get; set; }

        /// <summary>
        /// Contact e-mail address, used in the user agent when a call is made to the EVE Online SSO
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// State of the application, can be used to resume state 
        /// of the application once the user returns from the EVE Online login page.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Initialize the SSO settings as declared in the Web.Config
        /// </summary>
        public SsoSettings()
        {
            State = GenerateStateGuid;
        }

        /// <summary>
        /// Initialize the SSO settings using custom settings
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="callbackUrl"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="contactEmail"></param>
        public SsoSettings(string baseUrl, string callbackUrl, string scopes, string clientId, string clientSecret, string contactEmail)
        {
            BaseUrl = baseUrl;
            CallbackUrl = callbackUrl;
            Scope = scopes;
            ClientId = clientId;
            ClientSecret = clientSecret;
            ContactEmail = contactEmail;
            State = GenerateStateGuid;
        }

        /// <summary>
        /// If the user hasn't been given a unique state ID, we generate a new ID and store in session.
        /// </summary>
        private string GenerateStateGuid
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
    }
}
