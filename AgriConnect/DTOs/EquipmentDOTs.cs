public class CreateEquipmentDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public EquipmentType Type { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; } = "";
    public DateTime ManufacturingDate { get; set; }
    public ListingType ListingType { get; set; }
    public Condition Condition { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
}

public class UpdateEquipmentDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public EquipmentType Type { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; } = "";
    public DateTime ManufacturingDate { get; set; }
    public ListingType ListingType { get; set; }
    public Condition Condition { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public bool IsAvailable { get; set; }
}

public class EquipmentResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public EquipmentType Type { get; set; }
    public decimal Price { get; set; }
    public string Location { get; set; } = "";
    public DateTime ManufacturingDate { get; set; }
    public ListingType ListingType { get; set; }
    public Condition Condition { get; set; }
    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
}