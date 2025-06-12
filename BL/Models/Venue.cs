namespace EsportManager.BL.Models
{
    public class Venue
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
