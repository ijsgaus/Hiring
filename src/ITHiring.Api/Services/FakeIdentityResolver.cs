using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ITHiring.Api.Services
{
    internal class FakeIdentityResolver : IIdentityResolver
    {
        public Task<ClaimsIdentity> Resolve(string userName, string password)
        {
            // DEMO CODE, DON NOT USE IN PRODUCTION!!!
            if (userName == "TEST" && password == "TEST123")
            {
                return Task.FromResult(new ClaimsIdentity(new GenericIdentity(userName, "Token"), new Claim[] { }));
            }

            // Account doesn't exists
            return Task.FromResult<ClaimsIdentity>(null);
        }
    }
}