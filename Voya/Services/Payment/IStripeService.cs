using Voya.Dtos.Payment;

namespace Voya.Services.Payment
{
    public interface IStripeService
    {
        Task<string> CreateBookingAndPaymentSessionAsync(PaymentRequestDto req, long userId);
    }
}
