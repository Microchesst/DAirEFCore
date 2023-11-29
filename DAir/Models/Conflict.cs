namespace DAir.Models
{
    public class Conflict
    {
        public int ConflictID { get; set; }

        public int PilotID { get; set; }
        public int EmployeeID { get; set; }

        public string Description { get; set; } // Added property for conflict description
    }
}