namespace DAir.Models
{
    public class CabinMember
    {
        public int CabinMemberID { get; set; }
        public int EmployeeID { get; set; }
        public string GeoLocation { get; set; }
        public string Certification { get; set; }

        // Navigation properties
        public Employee Employee { get; set; }
        public ICollection<Languages> Languages { get; set; }
    }
}
