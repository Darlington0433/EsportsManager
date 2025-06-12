namespace EsportManager.BL.Models
{
    public class TeamSponsor
    {
        public int TeamId { get; set; }
        public Team Team { get; set; }
        public int SponsorId { get; set; }
        public Sponsor Sponsor { get; set; }
    }
}
