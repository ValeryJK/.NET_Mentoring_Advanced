using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;

namespace Catalog.API.Infrastructure
{
	/// <summary>
	/// Middleware for logging claims from an access token.
	/// </summary>
	public class TokenLoggingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<TokenLoggingMiddleware> _logger;

		/// <summary>
		/// Constructor for initializing the middleware with a request delegate and logger.
		/// </summary>
		/// <param name="next">The next middleware component in the pipeline.</param>
		/// <param name="logger">Logger instance for logging token claims.</param>
		public TokenLoggingMiddleware(RequestDelegate next, ILogger<TokenLoggingMiddleware> logger)
		{
			_next = next;
			_logger = logger;
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

				_logger.LogInformation("Access Token Claims:");
				foreach (var claim in token.Claims)
				{
					_logger.LogInformation("Claim Type: {ClaimType}, Claim Value: {ClaimValue}", claim.Type, claim.Value);

				}
			}

			await _next(context);
		}
	}
}