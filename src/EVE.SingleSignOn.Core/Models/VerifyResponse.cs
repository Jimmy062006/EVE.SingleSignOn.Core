using System;
using System.Collections.Generic;
using System.Text;

namespace EVE.SingleSignOn.Core
{
    public class VerifyResponse
    {
        public long CharacterId { get; set; }
        public string CharacterName { get; set; }
        public string CharacterOwnerHash { get; set; }
        public DateTime ExpiresOn { get; set; }
        public string Scopes { get; set; }
        public string TokenType { get; set; }
        public string IntellectualProperty { get; set; }
    }
}
