using AgriConnect.DTOs;

namespace AgriConnect.Services;

public interface IBlogService
{
    Task<IEnumerable<BlogResponseDto>> GetAllAsync(string? category);
    Task<BlogResponseDto?> GetByIdAsync(int id);
    Task<BlogResponseDto> CreateAsync(CreateBlogDto dto, int userId);
    Task<BlogResponseDto?> UpdateAsync(int blogId, UpdateBlogDto dto, int requestingUserId, bool isAdmin);
    Task<bool> DeleteAsync(int blogId, int requestingUserId, bool isAdmin);

    // Comments
    Task<CommentResponseDto> AddCommentAsync(int blogId, AddCommentDto dto, int userId, string userName);
    Task<bool> DeleteCommentAsync(int commentId, bool isAdmin);
}