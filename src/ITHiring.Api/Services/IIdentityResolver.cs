using System.Security.Claims;
using System.Threading.Tasks;

namespace ITHiring.Api.Services
{
    /// <summary>
    /// Resolve user name and password to <see cref="ClaimsIdentity"/>
    /// </summary>
    public interface IIdentityResolver
    {
        /// <summary>
        /// Resolve user name and password to <see cref="ClaimsIdentity"/> or null
        /// </summary>
        /// <param name="userName">user name</param>
        /// <param name="password">password</param>
        /// <returns>identity or null</returns>
        Task<ClaimsIdentity> Resolve(string userName, string password);
    }
}