namespace EsportManager.BL.Models
{
    public class Match
    {
        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public DateTime MatchDate { get; set; }
        public string Location { get; set; }
        public ICollection<MatchResult> Results { get; set; }
    }
}
