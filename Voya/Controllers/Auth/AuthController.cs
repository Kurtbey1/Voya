using Microsoft.AspNetCore.Mvc;
using Voya.Services.Auth;
using Voya.Dtos;
using Voya.Dtos.Auth;

namespace Voya.Controllers.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ILoginService _login;
        public AuthController(ILoginService login)
        {
            _login = login ?? throw new ArgumentNullException(nameof(login));

        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody]LoginReqDto req)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _login.LoginAsync(req);

            if (!result.IsSuccess)
            {
                return Unauthorized(new { message = result.Error });
            }

            return Ok(result.Value);

        }
      
    }
}
