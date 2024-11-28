using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;

namespace Cart.API.Infrastructure
{
    /// <summary>
    /// Middleware for logging claims from an access token.
    /// </summary>
    public class TokenLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<TokenLoggingMiddleware> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenLoggingMiddleware"/> class.
        /// Constructor for initializing the middleware with a request delegate and logger.
        /// </summary>
        /// <param name="next">The next middleware component in the pipeline.</param>
        /// <param name="logger">Logger instance for logging token claims.</param>
        public TokenLoggingMiddleware(RequestDelegate next, ILogger<TokenLoggingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        /// <summary>
        /// Middleware invocation logic. Retrieves and logs claims from the access token.
        /// </summary>
        /// <param name="context">HTTP context for the current request.</param>
        /// <returns>A task that represents the completion of the request.</returns>
        public async Task Invoke(HttpContext context)
        {
            var accessToken = await context.GetTokenAsync("access_token");

            if (!string.IsNullOrEmpty(accessToken))
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadJwtToken(accessToken);

                this.logger.LogInformation("Access Token Claims:");
                foreach (var claim in token.Claims)
                {
                    this.logger.LogInformation("Claim Type: {ClaimType}, Claim Value: {ClaimValue}", claim.Type, claim.Value);
                }
            }

            await this.next(context);
        }
    }
}