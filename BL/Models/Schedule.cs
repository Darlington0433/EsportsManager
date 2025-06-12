namespace EsportManager.BL.Models
{
    public class Schedule
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public Tournament Tournament { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
