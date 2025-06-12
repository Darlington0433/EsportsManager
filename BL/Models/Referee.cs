namespace EsportManager.BL.Models
{
    public class Referee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Match> Matches { get; set; }
    }
}
