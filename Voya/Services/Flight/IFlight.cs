
using Voya.Dtos.Flight;
namespace Voya.Services.Flight
{
    public interface IFlight
    {
        Task<CreateFlightRes> CreateFlightAsync(CreateFlightAdminReq req);
    }
}
