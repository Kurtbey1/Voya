using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Voya.Data;
using Voya.Dtos.Auth;
using Voya.Models;
using Voya.Options;

namespace Voya.Services.Auth
{
    public class TokenService(
        IOptions<JwtSettings> jwtOptions,
        ILogger<TokenService> logger,
        IDistributedCache cache,
        AppDbContext context) : ITokenService
    {
        private readonly JwtSettings _jwt = jwtOptions.Value;

        public async Task<TokenResponseDto> GenerateTokenAsync(User user)
        {
            var accessToken = CreateToken(user);
            var refreshToken = Guid.NewGuid().ToString();
            var cacheKey = $"RefreshToken:{user.User_ID}";

            await cache.SetStringAsync(cacheKey, refreshToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
            });

            logger.LogInformation("Refresh token generated for user: {UserId}", user.User_ID);

            return new TokenResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public string CreateToken(User user)
        {
            if (string.IsNullOrEmpty(_jwt.Key))
            {
                logger.LogCritical("JWT Signing Key is missing!");
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            var userIdValue = user.User_ID.ToString();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, userIdValue),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new(JwtRegisteredClaimNames.Sub, userIdValue),
                new("Role", user.Role ?? "Customer")
            };

            var keyBytes = Encoding.UTF8.GetBytes(_jwt.Key);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<TokenResponseDto> Refresh(TokenRequestDto request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            var userIdStr = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdStr) || !long.TryParse(userIdStr, out var userId))
            {
                throw new SecurityTokenException("Invalid token: User ID not found.");
            }

            var storedRefreshToken = await cache.GetStringAsync($"RefreshToken:{userId}");

            if (storedRefreshToken is null || storedRefreshToken != request.RefreshToken)
            {
                throw new SecurityTokenException("Invalid or expired Refresh Token!");
            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.User_ID == userId)
                ?? throw new SecurityTokenException("User not found");

            return await GenerateTokenAsync(user);
        }

        public async Task RevokeTokenAsync(string userId)
        {
            await cache.RemoveAsync($"RefreshToken:{userId}");
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm");
            }

            return principal;
        }
    }
}