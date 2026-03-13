using Voya.Dtos.Hotel;
using Voya.Services.Common;
namespace Voya.Services.HotelServices
{
    public interface IHotelService
    {
        // Client function
        Task <Result<IEnumerable<HotelResponseDto>>>GetActiveHotelsAsync();


        // Admin functions

        Task<Result<IEnumerable<HotelResponseDto>>> GetAllForAdminAsync();
        Task<Result<HotelResponseDto>>GetDetailsAsync(long id);
        Task<Result<HotelResponseDto>> CreateHotelAsync(CreateHotelDto req);
        Task<Result<bool>> UpdateHotelAsync(long id, CreateHotelDto dto);
        Task<Result<bool>> ToggleHotelStatusAsync(long id);



    }
}
