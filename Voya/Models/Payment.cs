using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Voya.Models
{
    public class Payment
    {
        public long Payment_ID { get; set; }

        public long Transaction_ID { get; set; }

        public Transaction Transaction { get; set; } = null!;

        public string Payment_Method { get; set; } = string.Empty;

        public Decimal  Amount { get; set; }
    }
}
