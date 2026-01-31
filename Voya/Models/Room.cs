using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voya.Models
{
    public class Room
    {
        public long Room_ID { get; set; }

        public long Hotel_ID { get; set; }

        public Hotel Hotel { get; set; } = null!;

        public string Room_Type { get; set; } = string.Empty;

        public decimal Price_Per_Night { get; set; }

        public int Base_Capacity { get; set; }

        public ICollection<RoomReservation> RoomReservations { get; set; } = [];
    }
}
