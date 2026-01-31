namespace Voya.Models
{
    public class Flight
    {
        public long Flight_ID { get; set; }

        public long Company_ID { get; set; }

        public string Plane { get; set; }= string.Empty;

        public string Flight_Status { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public DateTime ScheduledDeparture {  get; set; }

        public DateTime ScheduledArrival { get; set; }

        public Airline_Company Company { get; set; } = null!;

        public ICollection<Ticket> Tickets { get; set; } = [];

    }
}
