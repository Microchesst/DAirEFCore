namespace DAir.Models
{
    public class Pilot
    {
        public int PilotID { get; set; }
        public int EmployeeID { get; set; }
        public string GeoLocation { get; set; }
        public string Certification { get; set; }
        public string Rank { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
        public ICollection<Rating> RatingsReceived { get; set; }

    }
}
