namespace Voya.Dtos.Payment
{
    public class PaymentRequestDto
    {
        public long HotelId { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }

        public decimal Amount { get; set; }
        public string HotelName { get; set; } = string.Empty;

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
