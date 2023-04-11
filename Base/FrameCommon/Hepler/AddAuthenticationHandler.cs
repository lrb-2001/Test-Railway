using FrameModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace FrameCommon.Hepler;

public class AddAuthenticationHandler : AuthenticationHandler<FrameConst>
{
    public AddAuthenticationHandler(IOptionsMonitor<FrameConst> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
: base(options, logger, encoder, clock)
    { }
    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Developer", out var keys))
        {
            return AuthenticateResult.NoResult();
        }

        var key = keys.FirstOrDefault();

        //验证对比
        if (key == AppSettings.app(new string[] { "JWT", "Developer" }))
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "My IO")
                };

            var identity = new ClaimsIdentity(claims, FrameConst.Developer);
            var identities = new List<ClaimsIdentity> { identity };
            var principal = new ClaimsPrincipal(identities);
            var ticket = new AuthenticationTicket(principal, FrameConst.Developer);

            return AuthenticateResult.Success(ticket);
        }

        return AuthenticateResult.NoResult();
    }
}
