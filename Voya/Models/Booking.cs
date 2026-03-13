namespace Voya.Models
{
    public class Booking
    {
        public long Booking_ID { get; set; }

        public DateTime Booking_Date { get; set; } = DateTime.UtcNow;

        public string Booking_State { get; set; } = "Pending";

        public int Adults_Number { get; set; }

        public int Children_Number { get; set; }

        public long User_ID { get; set; }

        public User User { get; set; } = null!;
        public long Hotel_ID { get; set; }
        public virtual Hotel Hotel { get; set; } = null!;
        public ICollection<Transaction> Transactions {get;set;} = [] ;



    }
}
