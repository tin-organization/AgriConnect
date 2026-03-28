using System.ComponentModel.DataAnnotations;

public enum Role
{
    Seller = 1,
    Buyer = 2, 
    Admin = 3
}

public class User
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = "";
    [Required]
    public string Email { get; set; } = "";
    [Required]
    public string Password { get; set; } = "";

    [Required]
    public string NID { get; set; }="";

    [Required]
    public Role Role { get; set; }

    [Required]
    public string Address { get; set; }="";
    [Required]
    public string City { get; set; } = "";
    [Required]
    public string PhoneNumber { get; set; } = "";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}