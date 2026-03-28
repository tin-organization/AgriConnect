using System.ComponentModel.DataAnnotations;

public enum ItemType
{
    Produce = 1,
    Equipment = 2
}

public enum OrderStatus
{
    Active = 1,
    Cancelled = 2
}

public class Order
{
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required]
    public ItemType ItemType { get; set; }

    public int? ProduceId { get; set; }
    public Produce? Produce { get; set; }

    public int? EquipmentId { get; set; }
    public Equipment? Equipment { get; set; }

    public double Quantity { get; set; }

    public decimal PriceSnapshot { get; set; }

    public decimal TotalAmount { get; set; }

    public ListingType? ListingType { get; set; }

    public OrderStatus Status { get; set; } = OrderStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}