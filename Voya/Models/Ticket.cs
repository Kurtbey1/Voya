namespace Voya.Models
{
    public class Ticket
    {
        public long Ticket_ID { get; set; }

        public long Flight_ID { get; set; }

        public Flight Flight { get; set; } = null!;

        public long Booking_ID { get; set; }

        public Booking Booking { get; set; } = null!;

        public string Ticket_Type { get; set; } = string.Empty;

        public string Seat_Number { get; set; }  = string.Empty;


    }
}
 