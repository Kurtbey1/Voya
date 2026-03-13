using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Voya.Dtos.Auth;
using Voya.Services.Auth;

namespace Voya.Controllers.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController(ILoginService login, ISignupService signup, ITokenService _tokenService) : ControllerBase
{
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] SignupReqDto req)
    {
        var result = await signup.SignUpAsync(req);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailDto req)
    {
        var result = await signup.VerifyEmailAsync(req);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginReqDto req)
    {
        var result = await login.LoginAsync(req);

        if (!result.IsSuccess)
            return Unauthorized(new { message = result.Error });

        return Ok(result.Value);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRequestDto req)
    {
        try
        {
            var result = await _tokenService.Refresh(req);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null) return BadRequest(new { message = "Invalid User Context" });

        await _tokenService.RevokeTokenAsync(userId);

        return Ok(new { message = "Logged out successfully" });
    }
}