using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Stripe;
using System.Security.Claims;
using System.Text;
using Voya.Data;
using Voya.Middleware;
using Voya.Models;
using Voya.Options;
using Voya.Services.Auth;
using Voya.Services.Auth.Validator;
using Voya.Services.Common;
using Voya.Services.Email;
using Voya.Services.HotelServices;
using Voya.Services.Payment;
using StripeTokenService = Stripe.TokenService;
System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.Configure<JwtSettings>(jwtSection);

var stripeSecretKey = builder.Configuration["Stripe:SecretKey"];
StripeConfiguration.ApiKey = stripeSecretKey;

var jwtSettings = jwtSection.Get<JwtSettings>();

if (jwtSettings?.Key != null)
{
    var key = Encoding.UTF8.GetBytes(jwtSettings.Key);
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero,
            RoleClaimType = "Role"
        };
    });
}

var connStr = builder.Configuration.GetConnectionString("DefaultConnection")
              ?? throw new InvalidOperationException("Missing connection string");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connStr));



builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "Voya_";
});

builder.Services.AddFluentValidationAutoValidation()
                .AddValidatorsFromAssemblyContaining<SignupRequestValidator>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ISignupService, SignupService>();
builder.Services.AddScoped<ITokenService, Voya.Services.Auth.TokenService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddSingleton<IIdGenerator>(new SnowflakeIdGenerator(machineId: 1));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseMiddleware<SecurityMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();