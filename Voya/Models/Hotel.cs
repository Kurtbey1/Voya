using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voya.Models
{
    public class Hotel
    {
        public long Hotel_ID { get; set; }

        public string Hotel_Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public int StarRating { get; set; }

        public decimal BasePricePerNight { get; set; }

        public string MainImageUrl { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
