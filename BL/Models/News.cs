namespace EsportManager.BL.Models
{
    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishedDate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
