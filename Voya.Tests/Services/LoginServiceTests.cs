using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Voya.Data;
using Voya.Dtos.Auth;
using Voya.Models;
using Voya.Services.Auth;
using Xunit.Abstractions;

namespace Voya.Tests.Services
{
    public class LoginServiceTests
    {
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AppDbContext _context;
        private readonly Mock<IPasswordHasher<User>> _mockPassword;
        private readonly Mock<ILogger<LoginService>> _mockLogger;
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            var option = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
            _context = new AppDbContext(option);
            _mockTokenService = new Mock<ITokenService>();
            _mockPassword = new Mock<IPasswordHasher<User>>();
            _mockLogger = new Mock<ILogger<LoginService>>();
            _loginService = new LoginService(_context, _mockLogger.Object, _mockTokenService.Object, _mockPassword.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnSuccess_WhenPasswordIscorrect()
        {
            var loginDto = new LoginReqDto { Email = "test@voya.com", Password = "" };

            var user = new User { User_ID = 1, Email = loginDto.Email, Password_Hash = loginDto.Password };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _mockTokenService.Setup(x => x.CreateToken(It.IsAny<User>()))
            .Returns("Fake_JWT_Token");
            _mockPassword.Setup(x => x.VerifyHashedPassword(It.IsAny<User>(), user.Password_Hash, loginDto.Password))
                .Returns(PasswordVerificationResult.Success);

            var result = await _loginService.LoginAsync(loginDto);

            Assert.NotNull(result);
            Assert.True(result.IsSuccess, "The login should have succeeded.");
            Assert.NotNull(result.Value);
            Assert.Equal("Fake_JWT_Token", result.Value.Token);
            Console.WriteLine($"The generated token is: {result.Value.Token}");

        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            var loginDto = new LoginReqDto { Email = "test@voya.com", Password = "" };

            var user = new User { User_ID = 1, Email = loginDto.Email, Password_Hash = loginDto.Password };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            _mockPassword.Setup(x => x.VerifyHashedPassword(It.IsAny<User>(), user.Password_Hash, loginDto.Password))
                .Returns(PasswordVerificationResult.Failed);

            var result = await _loginService.LoginAsync(loginDto);

            Assert.NotNull(result);
            Assert.False(result.IsSuccess, "The login should have fail.");
            Assert.Null(result.Value);

        }



    }
} 
