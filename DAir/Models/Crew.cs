namespace DAir.Models
{
    public class Crew
    {
        public int CrewID { get; set; }
        public int Pilot { get; set; }
        public int CoPilot { get; set; }
        public int Pursuer { get; set; }
        public int FlightAttendant { get; set; }

        // Navigation properties
        public Employee PilotEmployee { get; set; }
        public Employee CoPilotEmployee { get; set; }
        public Employee PursuerEmployee { get; set; }
        public Employee FlightAttendantEmployee { get; set; }
    }

}
