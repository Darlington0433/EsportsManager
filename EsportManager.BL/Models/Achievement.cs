namespace EsportManager.Models
{
    public class Achievement
    {
        public int Id { get; set; }
        public int TeamId { get; set; }
        public int TournamentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime AchievedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}