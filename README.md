```csharp
using EVE.SingleSignOn.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EVE.SSO.LoginTest.Controllers
{
    public class LoginController : Controller
    {
        private ISingleSignOnClient _client;

        public LoginController(ISingleSignOnClient client)
        {
            // Client is imported with dependency injection
            // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.2
            _client = client;
        }

        public IActionResult Login()
        {
            // OPTIONAL: If there is any redirect to be saved, 
            // do that before hitting the redirect
            
            // With client ID & secret...
            var authUrl = _client.GetAuthenticationUrl(new Uri("https://login.eveonline.com/v2/oauth/authorize"), "myClientId", "SomeScopeHere", "https://{hostname}/login/callback", "Some-State-Parameter");

            // PKCE alternative (https://auth0.com/docs/api-auth/tutorials/authorization-code-grant-pkce)
            // authUrl = _client.GetAuthenticationUrl(new Uri("https://login.eveonline.com/v2/oauth/authorize"), "myClientId", "SomeScopeHere", "https://{hostname}/login/callback", "Some-State-Parameter", "Base64EncodedChallenge");

            // Redirect user to CCP's SSO
            return Redirect(authUrl);
        }

        public async Task<IActionResult> Callback(string code, string state)
        {
            // You should be validating that the state is correct here! Optional but recommended.

            // Validate the code we just received
            // The response data should be stored somehow (authentication cookie / .net core identity)
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?tabs=aspnetcore2x
            Tokens tokens = await _client.AuthorizeAsync(new Uri("https://login.eveonline.com/v2/oauth/token"), "clientId", "clientSecret", code).ConfigureAwait(false);

            // PKCE alternative (https://auth0.com/docs/api-auth/tutorials/authorization-code-grant-pkce)
            // tokens = await _client.AuthorizePKCEAsync(new Uri("https://login.eveonline.com/v2/oauth/token"), "clientId", "verifier", code).ConfigureAwait(false);

            // Information in Tokens includes a refresh token if any scope was requested
            // To refresh information, just pass the refresh token to the RefreshAsync method
            // Example: Tokens refresh = await _client.RefreshAsync(new Uri("https://login.eveonline.com/v2/oauth/token"), "clientId", "clientSecret", refreshToken).ConfigureAwait(false);

            // OPTIONAL: Redirect to a specific location as specified in Login step
            return RedirectToAction("Index", "Home");
        }
    }
}
```