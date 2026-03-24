using System.ComponentModel.DataAnnotations;

public enum EquipmentType
{
    Tractor = 1,
    Harvester = 2,
    Tillage = 3,
    Irrigation = 4,
    Seeding = 5
}

public enum ListingType
{
    ForRent = 1,
    ForSale = 2
}

public enum Condition
{
    New = 1,
    Used = 2
}

public class Equipment
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    [Required]
    public EquipmentType Type { get; set; }

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Location { get; set; } = "";

    [Required]
    public DateTime ManufacturingDate { get; set; }

    [Required]
    public ListingType ListingType { get; set; }

    [Required]
    public Condition Condition { get; set; }

    public string Brand { get; set; } = "";
    public string Model { get; set; } = "";
    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}