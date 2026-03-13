using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Voya.Data;
using Voya.Dtos.Payment;
using Voya.Models;
using Voya.Services.Common;

namespace Voya.Services.Payment
{
    public class StripeService(AppDbContext context, IHttpContextAccessor httpContextAccessor, IIdGenerator idGenerator, IConfiguration config) : IStripeService
    {
        private readonly AppDbContext _context = context;
        private readonly IIdGenerator _idGenerator = idGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<string> CreateBookingAndPaymentSessionAsync(PaymentRequestDto req, long userId)
        {
            StripeConfiguration.ApiKey = config.GetSection("Stripe:SecretKey").Value;

            var userPrincipal = _httpContextAccessor.HttpContext?.User;
            var claimUserId = userPrincipal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                              ?? userPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(claimUserId) || !long.TryParse(claimUserId, out var parsedUserId) || parsedUserId == 0)
                throw new Exception("Unauthorized: Invalid User ID extracted from token.");

            var dbUser = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.User_ID == parsedUserId)
                ?? throw new Exception("User not found in the clean database.");

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var newBooking = new Voya.Models.Booking
                {
                    Booking_ID = _idGenerator.NextId(),
                    User_ID = dbUser.User_ID,
                    Hotel_ID = req.HotelId,
                    Adults_Number = req.Adults,
                    Children_Number = req.Children,
                    Booking_State = "Pending",
                    Booking_Date = DateTime.UtcNow
                };

                _context.Bookings.Add(newBooking);
                await _context.SaveChangesAsync();

                var transaction = new Voya.Models.Transaction 
                {
                    Transaction_ID = _idGenerator.NextId(), 
                    Booking_ID = newBooking.Booking_ID,
                    Transaction_Type = "HotelBooking",
                    Transaction_Status = "Pending",
                    Transaction_Date = DateTime.UtcNow,
                    Provider_Ref = "Stripe"
                };

                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = ["card"],
                    LineItems = [
                        new() {
                            PriceData = new() {
                                UnitAmount = (long)(req.Amount * 100),
                                Currency = "usd",
                                ProductData = new() { Name = $"Booking at: {req.HotelName}" }
                            },
                            Quantity = 1
                        }
                    ],
                    Mode = "payment",
                    SuccessUrl = $"https://localhost:7188/api/payments/success?session_id={{CHECKOUT_SESSION_ID}}&transactionId={transaction.Transaction_ID}",
                    CancelUrl = "https://localhost:7188/api/payments/cancel",
                    Metadata = new Dictionary<string, string>
                    {
                        ["TransactionId"] = transaction.Transaction_ID.ToString(),
                        ["BookingId"] = newBooking.Booking_ID.ToString()
                    }
                };

                var session = await new SessionService().CreateAsync(options);
                await dbTransaction.CommitAsync();
                return session.Url;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception($"Stripe DB Error: {ex.Message}", ex);
            }
        }
    }
}