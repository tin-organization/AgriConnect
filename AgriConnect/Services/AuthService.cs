using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public interface IAuthService
{
    Task<(bool Success, string Message, UserResponseDto? Data)> RegisterAsync(RegisterDto dto);
    Task<(bool Success, string Message, string? Token)> LoginAsync(LoginDto dto);
    Task<(bool Success, string Message, UserResponseDto? Data)> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<(bool Success, string Message, UserResponseDto? Data)> GetProfileAsync(int userId);
    Task<(bool Success, string Message, List<UserResponseDto>? Data)> SearchUsersAsync(string query);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<(bool, string, UserResponseDto?)> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return (false, "Email already exists.", null);

        if (await _db.Users.AnyAsync(u => u.NID == dto.NID))
            return (false, "NID already registered.", null);

        var user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            NID = dto.NID,
            Role = dto.Role,
            Address = dto.Address,
            City = dto.City,
            PhoneNumber = dto.PhoneNumber
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return (true, "User registered successfully.", ToDto(user));
    }

    public async Task<(bool, string, string?)> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            return (false, "Invalid email or password.", null);

        var token = GenerateJwt(user);
        return (true, "Login successful.", token);
    }

    public async Task<(bool, string, UserResponseDto?)> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return (false, "User not found.", null);

        user.Name = dto.Name;
        user.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        user.NID = dto.NID;
        user.Address = dto.Address;
        user.City = dto.City;
        user.PhoneNumber = dto.PhoneNumber;
        user.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return (true, "Profile updated.", ToDto(user));
    }

    public async Task<(bool, string, UserResponseDto?)> GetProfileAsync(int userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return (false, "User not found.", null);
        return (true, "Success.", ToDto(user));
    }

    public async Task<(bool, string, List<UserResponseDto>?)> SearchUsersAsync(string query)
    {
        var users = await _db.Users
            .Where(u => u.Name.Contains(query) || u.Email.Contains(query) || u.City.Contains(query))
            .Select(u => ToDto(u))
            .ToListAsync();

        return (true, "Success.", users);
    }

    private string GenerateJwt(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static UserResponseDto ToDto(User u) => new()
    {
        Id = u.Id,
        Name = u.Name,
        Email = u.Email,
        NID = u.NID,
        Role = u.Role,
        Address = u.Address,
        City = u.City,
        PhoneNumber = u.PhoneNumber,
        CreatedAt = u.CreatedAt
    };
}