namespace DAir.Models
{
    public class FlightSchedule
    {
        public string FlightCode { get; set; }
        public int EmployeeID { get; set; }

        // Navigation properties
        public Flight Flight { get; set; }
        public Employee Employee { get; set; }
    }
}
