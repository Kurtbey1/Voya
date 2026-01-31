using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Voya.Models
{
    public class Hotel
    {
        public long Hotel_ID { get; set; }

        public string Hotel_Name { get; set; } = string.Empty;

        public ICollection<Room> Rooms { get; set; } = [];

        

    } 
}
