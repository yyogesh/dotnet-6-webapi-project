using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using ASPPRODUCT.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ASPPRODUCT.Handler;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly LearnContext _learnContext;
    public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder urlEncoder, ISystemClock clock, LearnContext learnContext) : base(options, logger, urlEncoder, clock)
    {
        _learnContext = learnContext;
    }

    protected async override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("No Header found");
        }
        var _headerValue = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
        var bytes = Convert.FromBase64String(_headerValue.Parameter);
        string credentials = Encoding.UTF8.GetString(bytes);
        if (!string.IsNullOrEmpty(credentials))
        {
            string[] array = credentials.Split(":");
            string username = array[0];
            string password = array[1];
            var user = await this._learnContext.TblUsers.FirstOrDefaultAsync(item => item.Userid == username && item.Password == password);
            if (user == null)
            {
                return AuthenticateResult.Fail("UnAuthorized");
            }

            // Generate token 

            var claim = new[] { new Claim(ClaimTypes.Name, username) };
            var identity = new ClaimsIdentity(claim, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
        else
        {
            return AuthenticateResult.Fail("UnAuthorized");
        }
    }
}