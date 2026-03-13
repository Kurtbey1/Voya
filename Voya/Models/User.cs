using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Voya.Models
{
    public class User
    {
        public long User_ID { get; set; }

        public string User_Name { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public string Password_Hash { get; set; } = string.Empty;

        public string Role { get; set; } = "Customer";
        public bool IsDeleted { get; set; } = false;
        public bool IsActive{ get; set; } = false;

        public string National_Number { get; set; } = string.Empty;


        public ICollection<Booking> Bookings { get; set; } = [];
    }
}
