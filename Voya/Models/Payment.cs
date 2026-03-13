using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Voya.Models
{
    public class Payment
    {
        public long Payment_ID { get; set; }

        public long Transaction_ID { get; set; }

        public Transaction Transaction { get; set; } = null!;
        public decimal Amount { get; set; }

        public string Payment_Method { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public string? Stripe_PaymentIntent_Id { get; set; }

        public DateTime Created_At { get; set; } = DateTime.UtcNow;
    }
}
  