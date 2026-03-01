public class RegisterDto
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public string NID { get; set; } = "";
    public Role Role { get; set; }
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
}

public class LoginDto
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}

public class UpdateProfileDto
{
    public string Name { get; set; } = "";
    public string Password { get; set; } = "";
    public string NID { get; set; } = "";
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
}

public class UserResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string NID { get; set; } = "";
    public Role Role { get; set; }
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}