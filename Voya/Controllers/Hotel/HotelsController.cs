using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using Voya.Services.HotelServices;

namespace Voya.Controllers.Hotel
{
    [ApiController]
    [Route("api/hotels")]
    [Authorize(Roles = "customer")]
    //client api
    public class HotelsController : Controller
    {
        private readonly HotelService _hotelService;
        public HotelsController(HotelService hotelService)
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
        }

        [HttpGet]
        public async Task<IActionResult> GetActive()
        {
            var result = await _hotelService.GetActiveHotelsAsync();
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetDetails(long id)
        {
            var result = await _hotelService.GetDetailsAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

    }
}
