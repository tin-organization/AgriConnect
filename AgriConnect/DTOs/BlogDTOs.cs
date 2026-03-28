namespace AgriConnect.DTOs;

// ── Blog ──────────────────────────────────────────────────────────────────────

public class CreateBlogDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class UpdateBlogDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
}

public class BlogResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CommentResponseDto> Comments { get; set; } = new();
}

// ── Comment ───────────────────────────────────────────────────────────────────

public class AddCommentDto
{
    public string Content { get; set; } = string.Empty;
}

public class CommentResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CommentatorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}