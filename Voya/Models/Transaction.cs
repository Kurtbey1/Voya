using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voya.Models
{

    public class Transaction
    {
        public long Transaction_ID { get; set; }

        public string Transaction_Type { get; set; } = string.Empty;

        public DateTime Transaction_Date { get; set; } = DateTime.UtcNow;

        public string   Provider_Ref { get; set; } = string.Empty;

        public string Transaction_Status { get; set; } = string.Empty;

        public long Booking_ID { get; set; }

        public Booking Booking { get; set; } = null!;

        public Payment Payment { get; set; } = null!;
    }
}
