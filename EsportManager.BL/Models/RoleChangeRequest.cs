namespace EsportManager.Models
{
    public class RoleChangeRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string RequestedRole { get; set; }
        public string Status { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}