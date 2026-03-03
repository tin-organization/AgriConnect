using System.ComponentModel.DataAnnotations;

public enum Category
{
    Vegetable = 1,
    Fruit = 2,
    Grain = 3
}

public enum Unit
{
    KG = 1,
    Grams = 2,
    Pound = 3
}

public class Produce
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = "";

    [Required]
    public string Description { get; set; } = "";

    [Required]
    public Category Category { get; set; }

    public decimal Price { get; set; }

    public Unit Unit { get; set; }

    public double AvailableUnitsLeft { get; set; }

    public string Location { get; set; } = "";

    public DateTime HarvestDate { get; set; }

    public DateTime ExpiryDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}