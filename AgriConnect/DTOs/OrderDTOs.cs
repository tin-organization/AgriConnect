public class CreateOrderDto
{
    public ItemType ItemType { get; set; }
    public int ItemId { get; set; }
    public double Quantity { get; set; }
}

public class UpdateOrderDto
{
    public double Quantity { get; set; }
}

public class OrderResponseDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public ItemType ItemType { get; set; }
    public int? ProduceId { get; set; }
    public int? EquipmentId { get; set; }
    public string ItemTitle { get; set; } = "";
    public double Quantity { get; set; }
    public decimal PriceSnapshot { get; set; }
    public decimal TotalAmount { get; set; }
    public ListingType? ListingType { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}