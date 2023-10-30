using System.ComponentModel.DataAnnotations;

namespace DAir.Models
{
    public class Flight
    {
        [Key]
        public string FlightCode { get; set; }
        public string DepartureAirport { get; set; }
        public string ArrivalAirport { get; set; }
        public string ScheduledDepartureTime { get; set; }
        public string ScheduledArrivalTime { get; set; }
        public int AircraftID { get; set; }
        public string State { get; set; }

        // Navigation properties
        public ICollection<FlightSchedule> FlightSchedules { get; set; }
    }
}
