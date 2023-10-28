namespace DAir.Models
{
    public class Rating
    {
        public int PilotID { get; set; }
        public int EmployeeID { get; set; }
        public int RatingValue { get; set; }

        // Navigation properties
        public Pilot Pilot { get; set; }
        public Employee Employee { get; set; }
    }
}
