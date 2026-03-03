using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using AgriConnect.DTOs;
using AgriConnect.Services;
using System.Security.Claims;

namespace AgriConnect.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlogController : ControllerBase
{
    private readonly IBlogService _blogService;
    private readonly IValidator<CreateBlogDto> _createValidator;
    private readonly IValidator<UpdateBlogDto> _updateValidator;
    private readonly IValidator<AddCommentDto> _commentValidator;

    public BlogController(
        IBlogService blogService,
        IValidator<CreateBlogDto> createValidator,
        IValidator<UpdateBlogDto> updateValidator,
        IValidator<AddCommentDto> commentValidator)
    {
        _blogService       = blogService;
        _createValidator   = createValidator;
        _updateValidator   = updateValidator;
        _commentValidator  = commentValidator;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private int GetUserId()  =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string GetUserName() =>
        User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

    private bool IsAdmin() =>
        User.FindFirstValue(ClaimTypes.Role) == "Admin"; 

    // ── GET /api/blog  (public – no auth required) ────────────────────────────
    /// <summary>Get all blogs. Filter by ?category=news</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll([FromQuery] string? category)
    {
        var blogs = await _blogService.GetAllAsync(category);
        return Ok(blogs);
    }

    // ── GET /api/blog/{id}  (public) ──────────────────────────────────────────
    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var blog = await _blogService.GetByIdAsync(id);
        if (blog is null) return NotFound(new { message = "Blog not found." });
        return Ok(blog);
    }

    // ── POST /api/blog  (logged-in users) ─────────────────────────────────────
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateBlogDto dto)
    {
        var validation = await _createValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        var blog = await _blogService.CreateAsync(dto, GetUserId());
        return CreatedAtAction(nameof(GetById), new { id = blog.Id }, blog);
    }

    // ── Post /api/blog/{id}  (owner only) ────────────────────────────────────
    [HttpPost("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateBlogDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        try
        {
            var updated = await _blogService.UpdateAsync(id, dto, GetUserId(), IsAdmin());
            if (updated is null) return NotFound(new { message = "Blog not found." });
            return Ok(updated);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // ── DELETE /api/blog/{id}  (owner or admin) ───────────────────────────────
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var deleted = await _blogService.DeleteAsync(id, GetUserId(), IsAdmin());
            if (!deleted) return NotFound(new { message = "Blog not found." });
            return Ok(new { message = "Blog deleted successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    // =========================================================================
    // COMMENTS
    // =========================================================================

    // ── POST /api/blog/{id}/comments  (logged-in users) ──────────────────────
    [HttpPost("{id:int}/comments")]
    [Authorize]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentDto dto)
    {
        var validation = await _commentValidator.ValidateAsync(dto);
        if (!validation.IsValid)
            return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

        try
        {
            var comment = await _blogService.AddCommentAsync(id, dto, GetUserId(), GetUserName());
            return Ok(comment);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // ── DELETE /api/blog/comments/{commentId}  (comment owner ) ───────
    [HttpDelete("comments/{commentId:int}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        try
        {
            var deleted = await _blogService.DeleteCommentAsync(commentId, IsAdmin());
            if (!deleted) return NotFound(new { message = "Comment not found." });
            return Ok(new { message = "Comment deleted successfully." });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}