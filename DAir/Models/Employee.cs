

namespace DAir.Models
{
    public class Employee
    {
        public Employee()
        {
            Pilots = new HashSet<Pilot>();
            CabinMembers = new HashSet<CabinMember>();
            RatingsGiven = new HashSet<Rating>();
            FlightSchedules = new HashSet<FlightSchedule>();
        }
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }

        // Navigation properties
        public ICollection<Pilot> Pilots { get; set; }
        public ICollection<CabinMember> CabinMembers { get; set; }
        public ICollection<Rating> RatingsGiven { get; set; }
        public ICollection<FlightSchedule> FlightSchedules { get; set; }
    }
}
