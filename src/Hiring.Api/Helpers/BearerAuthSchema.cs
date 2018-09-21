using Swashbuckle.AspNetCore.Swagger;

namespace Hiring.Api.Helpers
{
    internal class BearerAuthScheme : SecurityScheme
    {
        public string AuthorizationUrl { get; }

        public BearerAuthScheme(string authorizationUrl)
        {
            AuthorizationUrl = authorizationUrl;
            Type = "bearer";
        }
    }
}