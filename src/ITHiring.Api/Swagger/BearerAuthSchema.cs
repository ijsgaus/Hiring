using Swashbuckle.AspNetCore.Swagger;

namespace ITHiring.Api.Swagger
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