namespace EsportManager.BL.Models
{
    public class Sponsor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Industry { get; set; }
        public ICollection<TeamSponsor> TeamSponsors { get; set; }
    }
}
