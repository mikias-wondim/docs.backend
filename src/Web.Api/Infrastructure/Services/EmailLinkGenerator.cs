using Application.Abstractions.Services.Email;
using Web.Api.Endpoints.Auth;

namespace Web.Api.Infrastructure.Services;

public class EmailLinkGenerator(
    LinkGenerator linkGenerator,
    IHttpContextAccessor httpContextAccessor)
    : IEmailLinkGenerator
{
    public string GenerateVerificationLink(string token)
    {
        HttpContext httpContext = httpContextAccessor.HttpContext ??
                                  throw new InvalidOperationException("No HTTP context available");

        string uri = linkGenerator.GetUriByName(
            httpContext,
            VerifyEmail.UriName,
            values: new { token }) ?? throw new InvalidOperationException("Could not generate verification link");

        return uri;
    }
}
