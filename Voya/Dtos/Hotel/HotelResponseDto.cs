namespace Voya.Dtos.Hotel
{
    public record HotelResponseDto(
            long Id,
            string Name,
            string Description,
            string Address,
            int StarRating,
            decimal BasePricePerNight,
            string MainImageUrl
        );
   
}
