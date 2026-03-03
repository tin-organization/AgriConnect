namespace AgriConnect.Models;

public class Comment
{
    public int Id { get; set; }
    public int BlogId { get; set; }
    public Blog Blog { get; set; } = null!;
    public int UserId { get; set; }          // FK – the logged-in commenter
    public string CommentatorName { get; set; } = string.Empty; // denormalized display name
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}