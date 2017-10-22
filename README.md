```csharp
using EVE.SingleSignOn.Core;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EVE.SSO.LoginTest.Controllers
{
    public class LoginController : Controller
    {
        private ISingleSignOnClient _sso;

        public LoginController()
        {
            // Example settings
            var settings = new SsoSettings("https://login.eveonline.com/", "https://{hostname}/login/callback", "", "exampleClientId", "exampleClientSecret", "your@email.comes.here.com");

            // Initialize the SSO cilent
            _sso = new SingleSignOnClient(settings);
        }

        public IActionResult Login()
        {
            // OPTIONAL: If there is any redirect to be saved, 
            // do that before hitting the redirect

            // Redirect user to CCP's SSO
            return Redirect(_sso.GetAuthenticationUrl());
        }

        public IActionResult Logout()
        {
            // Handle logging out & redirecting where you want
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Callback(string code)
        {
            // Validate the code we just received
            // The response data should be stored somehow (authentication cookie / .net core identity)
            // https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?tabs=aspnetcore2x
            SsoResponse authentication = await _sso.AuthenticateAsync(code);

            // Information in SsoResponse includes a refresh token if any scope was requested
            // To refresh information, just pass the refresh token to the RefreshAsync method
            // Example: SsoResponse refresh = await _sso.RefreshAsync(authentication.RefreshToken);

            // Use the access token to get information about the character that logged in
            SsoCharacter character = await _sso.VerifyAsync(authentication.AccessToken);

            // OPTIONAL: Redirect to a specific location as specified in Login step
            return RedirectToAction("Index", "Home");
        }
    }
}
```