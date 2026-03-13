using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Voya.Dtos.Hotel;
using Voya.Services.Common;
using Voya.Services.HotelServices;

namespace Voya.Controllers.Hotel
{
    // Admin API for managing hotels
    [ApiController]
    [Route("api/cms/hotels")]
    [Produces("application/json")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    public class HotelCmsController : ControllerBase
    {
        private readonly IHotelService _hotelService;
        public HotelCmsController(IHotelService hotelService) 
        {
            _hotelService = hotelService ?? throw new ArgumentNullException(nameof(hotelService));
        }

        [HttpGet("{id:long}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await _hotelService.GetDetailsAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateHotelDto dto)
        {
            var result = await _hotelService.CreateHotelAsync(dto);
            if (!result.IsSuccess || result.Value == null)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result);
        }

        [HttpPut("{id:long}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] 
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(long id, [FromBody] CreateHotelDto dto)
        {
            var result = await _hotelService.UpdateHotelAsync(id, dto);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> ToggleStatus(long id)
        {
            var result = await _hotelService.ToggleHotelStatusAsync(id);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
        [HttpGet("test")]
        public IActionResult Get() => Ok("Success!");


    }
}
