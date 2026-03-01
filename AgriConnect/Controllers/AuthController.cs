using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IValidator<RegisterDto> _registerValidator;
    private readonly IValidator<LoginDto> _loginValidator;
    private readonly IValidator<UpdateProfileDto> _updateValidator;

    public AuthController(
        IAuthService authService,
        IValidator<RegisterDto> registerValidator,
        IValidator<LoginDto> loginValidator,
        IValidator<UpdateProfileDto> updateValidator)
    {
        _authService = authService;
        _registerValidator = registerValidator;
        _loginValidator = loginValidator;
        _updateValidator = updateValidator;
    }

    // POST /api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var validation = await _registerValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var (success, message, data) = await _authService.RegisterAsync(dto);
        return success ? Ok(new { message, data }) : BadRequest(new { message });
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var validation = await _loginValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var (success, message, token) = await _authService.LoginAsync(dto);
        return success ? Ok(new { message, token }) : Unauthorized(new { message });
    }

    // PUT /api/auth/profile
    [Authorize]
    [HttpPost("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var validation = await _updateValidator.ValidateAsync(dto);
        if (!validation.IsValid) return BadRequest(validation.Errors);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, message, data) = await _authService.UpdateProfileAsync(userId, dto);
        return success ? Ok(new { message, data }) : NotFound(new { message });
    }

    // GET /api/auth/profile
    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, message, data) = await _authService.GetProfileAsync(userId);
        return success ? Ok(new { message, data }) : NotFound(new { message });
    }

    // GET /api/auth/users?query=john
    [Authorize]
    [HttpGet("users")]
    public async Task<IActionResult> SearchUsers([FromQuery] string query = "")
    {
        var (_, message, data) = await _authService.SearchUsersAsync(query);
        return Ok(new { message, data });
    }
}