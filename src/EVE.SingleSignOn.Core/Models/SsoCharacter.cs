using Newtonsoft.Json;
using System;

namespace EVE.SingleSignOn.Core
{
    public class SsoCharacter
    {
        /// <summary>
        /// Character Identifier
        /// </summary>
        [JsonProperty("CharacterID")]
        public long CharacterId { get; set; }

        /// <summary>
        /// Name of the selected character
        /// </summary>
        [JsonProperty("CharacterName")]
        public string CharacterName { get; set; }

        /// <summary>
        /// Unique hash of the user, the character and salts and secrets all done in secret.
        /// The CharacterOwnerHash will change if the character is transferred to a different user account.
        /// </summary>
        [JsonProperty("CharacterOwnerHash")]
        public string CharacterOwnerHash { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("ExpiresOn")]
        public DateTime ExpiresOn { get; set; }

        /// <summary>
        /// Scopes that had been requested for the SSO
        /// </summary>
        [JsonProperty("Scopes")]
        public string Scopes { get; set; }

        /// <summary>
        /// This type should be Character
        /// </summary>
        [JsonProperty("TokenType")]
        public TokenType TokenType { get; set; }
    }
}
