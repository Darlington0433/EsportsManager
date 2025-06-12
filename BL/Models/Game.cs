namespace EsportManager.BL.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public ICollection<Tournament> Tournaments { get; set; }
    }
}
