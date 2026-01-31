using Voya.Dtos.Booking;
using Voya.Services.Common;
namespace Voya.Services.Booking
{
    public interface IBookingService
    {
        Task<Result<BookingResDto>> CreateBookingAsync(long user_Id ,BookingReqDto req);
    }
}
