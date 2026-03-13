namespace Voya.Dtos.Hotel
{
    public record CreateHotelDto(
        string Name,
        string Description,
        string Address,
        int StarRating,
        decimal Price,
        string ImageUrl);
    
}
