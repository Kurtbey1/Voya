using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Voya.Data;
using Voya.Dtos.Auth;
using Voya.Models;
using Voya.Services.Auth;
using Voya.Services.Common; // تأكد من استدعاء الـ Result DTO
using BCrypt.Net;

namespace Voya.Tests.Services
{
    public class LoginServiceTests
    {
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AppDbContext _context;
        private readonly Mock<ILogger<LoginService>> _mockLogger;
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

            _context = new AppDbContext(options);
            _mockTokenService = new Mock<ITokenService>();
            _mockLogger = new Mock<ILogger<LoginService>>();

            // التعديل: تمرير 3 باراميترات فقط كما هو موجود في الكود الفعلي الآن
            _loginService = new LoginService(_context, _mockLogger.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnSuccess_WhenPasswordIsCorrect()
        {
            // Arrange
            var rawPassword = "Password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(rawPassword); // استخدام BCrypt الحقيقي للـ Setup

            var loginDto = new LoginReqDto { Email = "test@voya.com", Password = rawPassword };
            var user = new User
            {
                User_ID = 1,
                Email = loginDto.Email,
                Password_Hash = hashedPassword,
                IsActive = true // يجب أن يكون النشط لكي ينجح اللوجن
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _mockTokenService.Setup(x => x.GenerateTokenAsync(It.IsAny<User>()))
                .ReturnsAsync(new TokenResponseDto { AccessToken = "Fake_JWT_Token", RefreshToken = "Fake_Refresh" });

            // Act
            var result = await _loginService.LoginAsync(loginDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Fake_JWT_Token", result.Value!.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnFailure_WhenPasswordIsWrong()
        {
            // Arrange
            var correctPassword = "CorrectPassword";
            var wrongPassword = "WrongPassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);

            var loginDto = new LoginReqDto { Email = "test@voya.com", Password = wrongPassword };
            var user = new User
            {
                User_ID = 1,
                Email = loginDto.Email,
                Password_Hash = hashedPassword,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _loginService.LoginAsync(loginDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Invalid Email or Password.", result.Error);
        }
    }
}