namespace DAir.Models
{
    public class Languages
    {
        public int CabinMemberID { get; set; }
        public string Language { get; set; }

        // Navigation property
        public CabinMember CabinMember { get; set; }
    }
}
