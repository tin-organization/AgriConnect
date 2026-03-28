using System.ComponentModel.DataAnnotations;

public class Income
{
    public int Id { get; set; }

    [Required]
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;

    public decimal Amount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}