using Microsoft.Extensions.Logging;
using Voya.Data;
using Voya.Dtos.Booking;
using Voya.Services.Common;
namespace Voya.Services.Booking
{
    public class BookingService
    {
        private readonly AppDbContext _context ;
        private readonly IIdGenerator _generator;
        private readonly ILogger<BookingService> _logger;

        public BookingService(AppDbContext context, IIdGenerator generator,ILogger<BookingService>logger)
        { 
            _context = context ?? throw new  ArgumentNullException(nameof(context));
            _generator = generator ?? throw new ArgumentNullException(nameof(generator));
            _logger = logger ?? throw new ArgumentNullException( nameof(logger));
        }

        public async Task <Result<BookingResDto>> CreateBookingAsync(long user_Id,BookingReqDto req)
        {
            _logger.LogInformation("Booking under process");
            if (req.Adults_Number <= 0)
            {
                _logger.LogWarning("Validation failed: Adult count is {AdultCount}", req.Adults_Number);
                return Result<BookingResDto>.Failure("At least one adult is required");
            }

            using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
                var booking = new Models.Booking
                {
                    Booking_ID = _generator.NextId(),   
                    Adults_Number = req.Adults_Number,
                    Children_Number = req.Children_Number,
                    Booking_State = "Pending",
                    Booking_Date = DateTime.UtcNow,
                    User_ID = user_Id,
                };

                var res = new BookingResDto
                {
                    Booking_ID = booking.Booking_ID,
                    Booking_State = booking.Booking_State,
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Result<BookingResDto>.Success(res);

            }

            catch(Exception ex)
            {
                await tx.RollbackAsync();
                _logger.LogError(ex, "Failed to create booking for user {UserId}", user_Id);
                return Result<BookingResDto>.Failure("An unexpected error occurred while saving your booking.");
            }

        }

    }
}
