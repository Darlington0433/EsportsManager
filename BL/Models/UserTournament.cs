namespace EsportManager.BL.Models
{
    public class UserTournament
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
        public string Role { get; set; }
    }
}
