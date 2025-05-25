namespace EsportManager.Models
{
    public class Registration
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int TournamentId { get; set; }
        public DateTime RegisteredAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}