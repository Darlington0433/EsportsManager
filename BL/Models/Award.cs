namespace EsportManager.BL.Models
{
    public class Award
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlayerId { get; set; }
        public Player Player { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
    }
}
