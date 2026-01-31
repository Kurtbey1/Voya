namespace Voya.Dtos.Flight
{
    public class CreateFlightAdminReq
    {
        public long Company_ID { get; set; }

        public string Plane { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public string Destination { get; set; } = string.Empty;

        public DateTime ScheduledDeparture { get; set; }

        public DateTime ScheduledArrival { get; set; }




    }
}
