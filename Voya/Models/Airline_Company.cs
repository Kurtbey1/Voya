namespace Voya.Models
{
    public class Airline_Company
    {
        public long Company_ID { get; set; }
        
        public string CompanyName { get; set; } = string.Empty;

        public Decimal Rate { get; set; }

        public ICollection<Flight> Flights { get; set; } = [];
    }
}
