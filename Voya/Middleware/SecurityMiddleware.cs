using Microsoft.Extensions.Options;
using Voya.Options;

namespace Voya.Middleware
{
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly JwtSettings _jwt;

        public SecurityMiddleware(RequestDelegate next, IOptions<JwtSettings> jwtOptions)
        {
            _next = next;
            _jwt = jwtOptions.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headerName = "X-Voya-Secret";

            if (!context.Request.Headers.TryGetValue(headerName, out var extractedValue) || extractedValue != _jwt.Key)
            {
                context.Response.StatusCode = 401;
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("Unauthorized: Security Header Mismatch!");
                return;
            }

            await _next(context);
        }
    }
}