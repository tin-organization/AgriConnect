public class CreateProduceDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Category Category { get; set; }
    public decimal Price { get; set; }
    public Unit Unit { get; set; }
    public double AvailableUnitsLeft { get; set; }
    public string Location { get; set; } = "";
    public DateTime HarvestDate { get; set; }
    public DateTime ExpiryDate { get; set; }
}

public class UpdateProduceDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Category Category { get; set; }
    public decimal Price { get; set; }
    public Unit Unit { get; set; }
    public double AvailableUnitsLeft { get; set; }
    public string Location { get; set; } = "";
    public DateTime HarvestDate { get; set; }
    public DateTime ExpiryDate { get; set; }
}

public class ProduceResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Category Category { get; set; }
    public decimal Price { get; set; }
    public Unit Unit { get; set; }
    public double AvailableUnitsLeft { get; set; }
    public double InitialUnitsLeft { get; set; }      // NEW
    public string Location { get; set; } = "";
    public DateTime HarvestDate { get; set; }
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}