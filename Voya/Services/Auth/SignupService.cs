using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Voya.Data;
using Voya.Dtos.Auth;
using Voya.Models;
using Voya.Services.Common;
using Voya.Services.Email;

namespace Voya.Services.Auth
{
    public class SignupService(
        AppDbContext context,
        IEmailService emailService,
        IDistributedCache cache,
        ILogger<SignupService> logger,
        IIdGenerator idGenerator) : ISignupService
    {
        public async Task<Result<SignupResDto>> SignUpAsync(SignupReqDto req)
        {
            logger.LogInformation("Attempt to register Account for Email: {Email}", req.Email);

            if (await context.Users.AnyAsync(u => u.Email == req.Email))
            {
                logger.LogWarning("Registration failed: Email {Email} already exists.", req.Email);
                return Result<SignupResDto>.Failure("This email is already registered.");
            }

            if (await context.Users.AnyAsync(u => u.Phone == req.Phone))
            {
                logger.LogWarning("Registration failed: Phone {Phone} already exists.", req.Phone);
                return Result<SignupResDto>.Failure("This Phone is already registered.");
            }

            var user = new User
            {
                User_ID = idGenerator.NextId(),
                User_Name = req.Name,
                IsActive = false,
                Email = req.Email,
                Password_Hash = BCrypt.Net.BCrypt.HashPassword(req.Password),
                Phone = req.Phone,
                Nationality = req.Nationality,
                Role = "Customer"
            };

            var verificationCode = new Random().Next(10000, 99999).ToString();

            await cache.SetStringAsync($"verify:{user.Email}", verificationCode, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            context.Users.Add(user);
            await context.SaveChangesAsync();

            await emailService.SendEmailAsync(user.Email, "Voya - Verification Code",
                $"Welcome to Voya! Your verification code is: <b>{verificationCode}</b>");

            return Result<SignupResDto>.Success(new SignupResDto
            {
                User_ID = user.User_ID,
                Name = user.User_Name,
                Email = user.Email,
                Message = "User registered successfully. Please verify your email."
            });
        }

        public async Task<Result<bool>> VerifyEmailAsync(VerifyEmailDto req)
        {
            var savedCode = await cache.GetStringAsync($"verify:{req.Email}");

            if (savedCode == null || savedCode != req.Code)
            {
                return Result<bool>.Failure("Invalid or expired verification code.");
            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == req.Email);
            if (user == null) return Result<bool>.Failure("User not found.");

            user.IsActive = true;
            await cache.RemoveAsync($"verify:{req.Email}");
            await context.SaveChangesAsync();

            return Result<bool>.Success(true);
        }
    }
}