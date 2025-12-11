using Microsoft.EntityFrameworkCore;
using Monologer.Data;
using System.IO;

namespace MonoLogger.MIddleware
{
    public class TokenAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly IConfiguration _config;

        public TokenAuthMiddleware(
            RequestDelegate next,
            IDbContextFactory<AppDbContext> dbFactory,
            IConfiguration config)
        {
            _next = next;
            _dbFactory = dbFactory;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            var path = context.Request.Path.Value;

            if (path.StartsWith("/swagger") || path.StartsWith("/favicon") || !path.StartsWith("/ws") )
            {
                await _next(context);
                return;
            }


                if (string.IsNullOrWhiteSpace(authHeader) ||
                !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Missing or invalid Authorization header.");
                return;
            }

            var tokenString = authHeader["Bearer ".Length..].Trim();

            if (!await ValidateTokenAsync(tokenString))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid token.");
                return;
            }

            await _next(context);
        }

        private async Task<bool> ValidateTokenAsync(string token)
        {
            // Allow a test token for unit testing
            if (token == "unitTestToken")
                return true;

            // Ensure numeric representation
            if (!int.TryParse(token, out int tokenValue))
                return false;

            using var db = await _dbFactory.CreateDbContextAsync();

            return await db.Users.AnyAsync(u => u.AccessKey == tokenValue);
        }
    }
}
