using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Voya.Models;
using Voya.Options;
namespace Voya.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwt;
        private readonly ILogger<TokenService> _logger;
        public TokenService(IOptions<JwtSettings> jwtOptions,ILogger<TokenService> logger)
        { 
            _jwt = jwtOptions.Value; 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        

        public string CreateToken(User user)
        {
            if (string.IsNullOrEmpty(_jwt.Key))
            {
                _logger.LogCritical("JWT Signing Key is missing in configuration!");
                throw new InvalidOperationException("JWT Key is not configured.");
            }

            _logger.LogInformation("Generating token for user: {UserId}", user.User_ID);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.User_ID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Role,user.Role)
            };

            var KeyBytes = Encoding.UTF8.GetBytes(_jwt.Key);
            var securityKey = new SymmetricSecurityKey(KeyBytes);

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
    }
}
