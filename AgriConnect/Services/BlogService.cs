using Microsoft.EntityFrameworkCore;
using AgriConnect.DTOs;
using AgriConnect.Models;

namespace AgriConnect.Services;

public class BlogService : IBlogService
{
    private readonly AppDbContext _db;

    public BlogService(AppDbContext db)
    {
        _db = db;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static BlogResponseDto MapBlog(Blog blog) => new()
    {
        Id          = blog.Id,
        UserId      = blog.UserId,
        AuthorName  = blog.User?.Name ?? string.Empty,
        Title       = blog.Title,
        Description = blog.Description,
        Category    = blog.Category,
        CreatedAt   = blog.CreatedAt,
        UpdatedAt   = blog.UpdatedAt,
        Comments    = blog.Comments.Select(MapComment).ToList()
    };

    private static CommentResponseDto MapComment(Comment c) => new()
    {
        Id              = c.Id,
        UserId          = c.UserId,
        CommentatorName = c.CommentatorName,
        Content         = c.Content,
        CreatedAt       = c.CreatedAt
    };

    private IQueryable<Blog> BlogsWithIncludes() =>
        _db.Blogs
           .Include(b => b.User)
           .Include(b => b.Comments);

    // ── Blog CRUD ─────────────────────────────────────────────────────────────

    public async Task<IEnumerable<BlogResponseDto>> GetAllAsync(string? category)
    {
        var query = BlogsWithIncludes().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(b => b.Category.ToLower() == category.ToLower());

        var blogs = await query.OrderByDescending(b => b.CreatedAt).ToListAsync();
        return blogs.Select(MapBlog);
    }

    public async Task<BlogResponseDto?> GetByIdAsync(int id)
    {
        var blog = await BlogsWithIncludes()
                         .AsNoTracking()
                         .FirstOrDefaultAsync(b => b.Id == id);
        return blog is null ? null : MapBlog(blog);
    }

    public async Task<BlogResponseDto> CreateAsync(CreateBlogDto dto, int userId)
    {
        var blog = new Blog
        {
            UserId      = userId,
            Title       = dto.Title,
            Description = dto.Description,
            Category    = dto.Category.ToLower(),
            CreatedAt   = DateTime.UtcNow
        };

        _db.Blogs.Add(blog);
        await _db.SaveChangesAsync();

        var created = await BlogsWithIncludes().FirstAsync(b => b.Id == blog.Id);
        return MapBlog(created);
    }

    public async Task<BlogResponseDto?> UpdateAsync(
        int blogId, UpdateBlogDto dto, int requestingUserId, bool isAdmin)
    {
        var blog = await BlogsWithIncludes().FirstOrDefaultAsync(b => b.Id == blogId);
        if (blog is null) return null;

        if (blog.UserId != requestingUserId)
            throw new UnauthorizedAccessException("Only the post owner can edit this blog.");

        if (dto.Title       != null) blog.Title       = dto.Title;
        if (dto.Description != null) blog.Description = dto.Description;
        if (dto.Category    != null) blog.Category    = dto.Category.ToLower();
        blog.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return MapBlog(blog);
    }

    public async Task<bool> DeleteAsync(int blogId, int requestingUserId, bool isAdmin)
    {
        var blog = await _db.Blogs.FindAsync(blogId);
        if (blog is null) return false;

        if (!isAdmin && blog.UserId != requestingUserId)
            throw new UnauthorizedAccessException("You are not allowed to delete this blog.");

        _db.Blogs.Remove(blog);
        await _db.SaveChangesAsync();
        return true;
    }

    // ── Comments ──────────────────────────────────────────────────────────────

    public async Task<CommentResponseDto> AddCommentAsync(
        int blogId, AddCommentDto dto, int userId, string userName)
    {
        var blogExists = await _db.Blogs.AnyAsync(b => b.Id == blogId);
        if (!blogExists) throw new KeyNotFoundException("Blog not found.");

        var comment = new Comment
        {
            BlogId          = blogId,
            UserId          = userId,
            CommentatorName = userName,
            Content         = dto.Content,
            CreatedAt       = DateTime.UtcNow
        };

        _db.Comments.Add(comment);
        await _db.SaveChangesAsync();
        return MapComment(comment);
    }

    public Task<bool> DeleteCommentAsync(int commentId, bool isAdmin)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteCommentAsync(int commentId, int requestingUserId, bool isAdmin)
    {
        var comment = await _db.Comments.FindAsync(commentId);
        if (comment is null) return false;

        if (!isAdmin && comment.UserId != requestingUserId)
            throw new UnauthorizedAccessException("You are not allowed to delete this comment.");

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();
        return true;
    }
}