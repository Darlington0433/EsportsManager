namespace EsportManager.Models
{
    public class TournamentResult
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public int TeamId { get; set; }
        public int Rank { get; set; }
        public decimal Prize { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}