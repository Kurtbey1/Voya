using Microsoft.AspNetCore.Identity;
using Voya.Models;
using Microsoft.EntityFrameworkCore;
using Voya.Data;
using Voya.Dtos.Auth;
using Voya.Services.Common;
namespace Voya.Services.Auth
{
    public class LoginService:ILoginService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<LoginService> _logger;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher<User> _passwordHasher;


        public LoginService  (AppDbContext context, ILogger<LoginService> logger, ITokenService tokenService, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _logger = logger;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
        }

        public async Task <Result<LoginResDto>> LoginAsync(LoginReqDto req) 
        {
            _logger.LogInformation("Logging for Email {Email} is under process", req.Email);

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == req.Email);

            if (user == null) 
            {
                _logger.LogWarning("LogIn failed because null valus");
                return Result<LoginResDto>.Failure("Email Or Password Are Required");
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password_Hash, req.Password);

            if (verificationResult == PasswordVerificationResult.Failed) 
            {
                _logger.LogWarning("Login attempt failed: Wrong password for {Email}.", req.Email);
                return Result<LoginResDto>.Failure("Invalid Email or Password.");
            }

            var token = _tokenService.CreateToken(user);

            return Result<LoginResDto>.Success(
                new LoginResDto
                {
                    Token = token,
                    Email = user.Email
                });
            


        }
    }
}
