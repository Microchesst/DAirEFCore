namespace DAir.Models
{
    public class Rating
    {
        public int PilotID { get; set; }
        public int EmployeeID { get; set; }
        public int RatingValue { get; set; }

        // For the rater (employee who is rating)
        public int RaterID { get; set; } // Foreign key property 
        public Employee Rater { get; set; }

        // For the ratee (pilot who is being rated)
        public int RateeID { get; set; } // Foreign key property (type should match Pilot's primary key)
        public Pilot Ratee { get; set; } // Navigation property



        // Navigation properties
        public Pilot Pilot { get; set; }
        public Employee Employee { get; set; }
    }
}
