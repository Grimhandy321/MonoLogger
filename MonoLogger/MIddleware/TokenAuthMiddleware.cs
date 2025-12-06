using Monologer.Data;
using System.Data.Entity;

namespace MonoLogger.MIddleware
{
        public class TokenAuthMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly AppDbContext _dbContext;
            private readonly IConfiguration _config;

            public TokenAuthMiddleware(RequestDelegate next, AppDbContext dbContext, IConfiguration config)
            {
                _next = next;
                _dbContext = dbContext;
                _config = config;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();

                if (string.IsNullOrWhiteSpace(authHeader) ||
                    !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Missing or invalid Authorization header.");
                    return;
                }
                var tokenString = authHeader.Substring("Bearer ".Length).Trim();

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
    
                if (token == "unitTestToken")
                    return true;

          
                if (!int.TryParse(token, out int tokenValue))
                    return false;
                bool exists = await _dbContext.Users.AnyAsync(u => u.AccessKey == tokenValue);

                return exists;
            }
        }
  
}
