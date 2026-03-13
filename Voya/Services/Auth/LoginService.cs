using Microsoft.EntityFrameworkCore;
using Voya.Data;
using Voya.Dtos.Auth;
using Voya.Services.Common;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace Voya.Services.Auth
{
    public class LoginService(AppDbContext context, ILogger<LoginService> logger, ITokenService tokenService) : ILoginService
    {
        public async Task<Result<LoginResDto>> LoginAsync(LoginReqDto req)
        {
            logger.LogInformation("Processing login for: {Email}", req.Email);

            var user = await context.Users.SingleOrDefaultAsync(u => u.Email == req.Email);

            if (user == null)
            {
                return Result<LoginResDto>.Failure("Invalid Email or Password.");
            }

            if (!user.IsActive)
            {
                return Result<LoginResDto>.Failure("User account is inactive.");
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(req.Password, user.Password_Hash);

            if (!isPasswordValid)
            {
                return Result<LoginResDto>.Failure("Invalid Email or Password.");
            }

            var tokenResponse = await tokenService.GenerateTokenAsync(user);

            return Result<LoginResDto>.Success(new LoginResDto
            {
                Token = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                Email = user.Email ?? string.Empty
            });
        }
    }
}