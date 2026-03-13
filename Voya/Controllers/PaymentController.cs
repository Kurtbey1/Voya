using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Security.Claims;
using Voya.Dtos.Payment;
using Voya.Services.Payment;

namespace Voya.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route("api/payments")]
    public class PaymentController : ControllerBase
    {
        private readonly IStripeService _stripeService;

        public PaymentController(IStripeService stripeService)
        {
            _stripeService = stripeService;
        }

        [HttpPost("create-checkout-session")]
        public async Task<IActionResult> CreateSession([FromBody] PaymentRequestDto req)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User not identified from token." });
            }

            if (!long.TryParse(userIdClaim.Value, out long userId))
            {
                return BadRequest(new { message = "Invalid User ID in token." });
            }

            try
            {
                var checkoutUrl = await _stripeService.CreateBookingAndPaymentSessionAsync(req, userId);

                return Ok(new { url = checkoutUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("success")]
        public IActionResult PaymentSuccess([FromQuery] string session_id, [FromQuery] long transactionId)
        {
            return Ok(new
            {
                message = "Payment redirect successful",
                session = session_id,
                t_id = transactionId
            });
        }

        [HttpGet("cancel")]
        public IActionResult PaymentCancel()
        {
            return BadRequest(new { message = "Payment was cancelled by the user." });
        }
    }
}