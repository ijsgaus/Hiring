using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using ITHiring.Api.Helpers;
using ITHiring.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ITHiring.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IIdentityResolver _resolver;
        private readonly TokenProviderOptions _options;

        public TokenController(IOptions<TokenProviderOptions> options, IIdentityResolver resolver)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _options = options.Value;
            TokenProviderOptions.ThrowIfNotConfigured(_options);
        }


        /// <summary>
        /// Request bearer token
        /// </summary>
        /// <param name="request">request parameters</param>
        /// <returns>bearer token description</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponse), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<TokenResponse>> Post([FromBody] TokenRequest request)
        {
            var identity = await _resolver.Resolve(request.UserName, request.Password);
            if (identity == null)
                return BadRequest("Invalid username or password.");
            var now = DateTime.UtcNow;

            // Specifically add the jti (nonce), iat (issued timestamp), and sub (subject/user) claims.
            // You can add other claims here, if you want:
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, request.UserName),
                //new Claim(JwtRegisteredClaimNames.Jti, await _options.NonceGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Create the JWT and write it to a string
            var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new TokenResponse(encodedJwt, _options.Expiration.TotalSeconds);
        }

       

        /// <summary>
        /// Response when success login
        /// </summary>
        public class TokenResponse
        {
            public TokenResponse(string token, double expired)
            {
                Token = token;
                Expired = expired;
            }

            /// <summary>
            /// Bearer token
            /// </summary>
            public string Token { get; }

            /// <summary>
            /// Expired in second
            /// </summary>
            public double Expired { get; }
        }

        /// <summary>
        /// Request for token
        /// </summary>
        public class TokenRequest
        {
            /// <summary>
            /// User name
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// User password
            /// </summary>
            public string Password { get; set; }
        }

        
    }
}