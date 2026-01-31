namespace Voya.Models
{
    public class RoomReservation
    {
        public long Reservation_ID { get; set; }
        
        public long Room_ID { get; set; }

        public long Booking_ID { get; set; }

        public DateTime Check_In { get; set; }

        public DateTime Check_Out { get; set; }

        public Booking Booking { get; set; } = null!;

        public Room Room { get; set; } = null!;



    }
}
