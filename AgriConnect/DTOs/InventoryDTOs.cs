public class InventoryDashboardDto
{
    public int TotalProduceListings { get; set; }
    public int TotalEquipmentListings { get; set; }
    public int TotalActiveOrders { get; set; }
    public decimal GrossRevenue { get; set; }
    public decimal PlatformCut { get; set; }
    public decimal NetRevenue { get; set; }
}

public class InventoryProduceDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public Category Category { get; set; }
    public decimal Price { get; set; }
    public Unit Unit { get; set; }
    public double AvailableUnitsLeft { get; set; }
    public double InitialUnitsLeft { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsLowStock { get; set; }
    public bool IsExpired { get; set; }
    public string Location { get; set; } = "";
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InventoryEquipmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public EquipmentType Type { get; set; }
    public decimal Price { get; set; }
    public ListingType ListingType { get; set; }
    public Condition Condition { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public bool IsAvailable { get; set; }
    public bool HasActiveOrder { get; set; }
    public string Location { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

public class InventoryOrderDto
{
    public int Id { get; set; }
    public ItemType ItemType { get; set; }
    public string ItemTitle { get; set; } = "";
    public double Quantity { get; set; }
    public decimal PriceSnapshot { get; set; }
    public decimal TotalAmount { get; set; }
    public ListingType? ListingType { get; set; }
    public OrderStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ToggleProduceAvailabilityDto
{
    public bool MakeAvailable { get; set; }
}