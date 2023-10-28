namespace DAir.Models
{
  
        public class Rating
        {
            public int RaterID { get; set; } // Foreign key for Employee
            public int RateeID { get; set; } // Foreign key for Pilot
            public int RatingValue { get; set; }

            // Navigation properties
            public Employee Rater { get; set; } // Employee who gave the rating
            public Pilot Ratee { get; set; } // Pilot who received the rating
        }
   
}
