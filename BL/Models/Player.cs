namespace EsportManager.BL.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Nationality { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
    }
}
