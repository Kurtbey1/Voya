using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Voya.Data;
using Voya.Dtos.Hotel;
using Voya.Models;
using Voya.Services.Common;
namespace Voya.Services.HotelServices
{
    public class HotelService: IHotelService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HotelService> _logger;
        private readonly IIdGenerator _snowflakeIdGenerator;
        public HotelService(AppDbContext context, ILogger<HotelService> logger, IIdGenerator snowflakeIdGenerator) 
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _snowflakeIdGenerator = snowflakeIdGenerator ?? throw new ArgumentNullException(nameof(snowflakeIdGenerator));  

        }
        // Client API - Get all active hotels
        public async Task<Result<IEnumerable<HotelResponseDto>>>GetActiveHotelsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all Active Hotels for the client");

                var hotels = await _context.Hotels
                    .AsNoTracking()
                    .Where(h => h.IsActive)
                    .Select(h => new HotelResponseDto
                    (
                        h.Hotel_ID,
                        h.Hotel_Name,
                        h.Description,
                        h.Address,
                        h.StarRating,
                        h.BasePricePerNight, 
                        h.MainImageUrl
                    )).ToListAsync();

                return Result<IEnumerable<HotelResponseDto>>.Success(hotels);
            }

            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching active hotels.");
                return Result<IEnumerable<HotelResponseDto>>.Failure("An error occurred while fetching active hotels.");
            }
        }

        // Admin API - Get all hotels regardless of status
        public async Task<Result<IEnumerable<HotelResponseDto>>> GetAllForAdminAsync()
        {
            var hotels = await _context.Hotels
                .AsNoTracking()
                .Select(h => new HotelResponseDto
                (
                    h.Hotel_ID,
                    h.Hotel_Name,
                    h.Description,
                    h.Address,
                    h.StarRating,
                    h.BasePricePerNight, 
                    h.MainImageUrl
                )).ToListAsync();

            return Result<IEnumerable<HotelResponseDto>>.Success(hotels);
        }

        // Get details of a specific hotel by ID (for both client and admin)
        public async Task<Result<HotelResponseDto>> GetDetailsAsync(long id)
        {
            if (id <=0)
            {
                return Result<HotelResponseDto>.Failure("Invalid Hotel ID");
            }
            try 
            {
                _logger.LogInformation("Fetching details for Hotel with ID: {HotelId}", id);
                var hotel = await _context.Hotels.AsNoTracking()
                    .Where(h => h.Hotel_ID == id && h.IsActive)
                    .Select(h => new HotelResponseDto
                    (
                        h.Hotel_ID,
                        h.Hotel_Name,
                        h.Description,
                        h.Address,
                        h.StarRating,
                        h.BasePricePerNight, 
                        h.MainImageUrl
                    )).FirstOrDefaultAsync();

                if (hotel == null)
                {
                    _logger.LogWarning("Hotel with ID: {HotelId} not found or is inactive.", id);
                    return Result<HotelResponseDto>.Failure("Hotel not found or is inactive.");
                }
                return Result<HotelResponseDto>.Success(hotel);
            }

            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching hotel details for ID: {HotelId}", id);
                return Result<HotelResponseDto>.Failure("An error occurred while fetching hotel details.");
            }
        }

        // Admin API - Create a new hotel
        public async Task<Result<HotelResponseDto>> CreateHotelAsync(CreateHotelDto req)
        {
            if (req == null)
            {
                _logger.LogWarning("CreateHotelAsync called with null request.");
                return Result<HotelResponseDto>.Failure("Request cannot be null.");
            }
            try
            {
                var newHotel = new Hotel
                {
                    Hotel_ID= _snowflakeIdGenerator.NextId(),
                    Hotel_Name = req.Name,
                    Description = req.Description,
                    Address = req.Address,
                    StarRating = req.StarRating,
                    BasePricePerNight = req.Price,
                    MainImageUrl = req.ImageUrl,
                    IsActive = true
                };
                await _context.Hotels.AddAsync(newHotel);
                await _context.SaveChangesAsync();

                return Result<HotelResponseDto>.Success(new HotelResponseDto
                (
                    newHotel.Hotel_ID,
                    newHotel.Hotel_Name,
                    newHotel.Description,
                    newHotel.Address,
                    newHotel.StarRating,
                    newHotel.BasePricePerNight, 
                    newHotel.MainImageUrl
                ));
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new hotel.");
                return Result<HotelResponseDto>.Failure("An error occurred while creating the hotel.");

            }
        }

        // Admin API - Update an existing hotel
        public async Task<Result<bool>> UpdateHotelAsync(long id, CreateHotelDto dto)
        {
            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.Hotel_ID == id);

            if (hotel == null) return Result<bool>.Failure("Hotel not found.");

            hotel.Description = dto.Description;
            hotel.Address = dto.Address;
            hotel.BasePricePerNight = dto.Price;
            hotel.StarRating = dto.StarRating;
            hotel.MainImageUrl = dto.ImageUrl;

            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        // Admin API - Toggle hotel active status
        public async Task<Result<bool>> ToggleHotelStatusAsync(long id)
        {
            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.Hotel_ID == id);

            if (hotel == null) return Result<bool>.Failure("Hotel not found.");

            hotel.IsActive = !hotel.IsActive;
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        private static HotelResponseDto MapToDto(Hotel h) =>
            new (h.Hotel_ID, h.Hotel_Name, h.Description, h.Address, h.StarRating, h.BasePricePerNight, h.MainImageUrl);
    }




}

