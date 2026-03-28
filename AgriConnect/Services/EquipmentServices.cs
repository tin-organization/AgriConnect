using Microsoft.EntityFrameworkCore;

public interface IEquipmentService
{
    Task<(bool Success, string Message, EquipmentResponseDto? Data)> CreateAsync(int sellerId, CreateEquipmentDto dto);
    Task<(bool Success, string Message, EquipmentResponseDto? Data)> GetByIdAsync(int id);
    Task<(bool Success, string Message, List<EquipmentResponseDto>? Data)> GetAllAsync();
    Task<(bool Success, string Message, List<EquipmentResponseDto>? Data)> SearchAsync(string query);
    Task<(bool Success, string Message, EquipmentResponseDto? Data)> UpdateAsync(int id, UpdateEquipmentDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}

public class EquipmentService : IEquipmentService
{
    private readonly AppDbContext _db;

    public EquipmentService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool, string, EquipmentResponseDto?)> CreateAsync(int sellerId, CreateEquipmentDto dto)
    {
        var equipment = new Equipment
        {
            SellerId = sellerId,            // NEW
            Title = dto.Title,
            Description = dto.Description,
            Type = dto.Type,
            Price = dto.Price,
            Location = dto.Location,
            ManufacturingDate = dto.ManufacturingDate,
            ListingType = dto.ListingType,
            Condition = dto.Condition,
            Brand = dto.Brand,
            Model = dto.Model,
        };

        _db.Equipments.Add(equipment);
        await _db.SaveChangesAsync();
        return (true, "Equipment listed successfully.", ToDto(equipment));
    }

    public async Task<(bool, string, EquipmentResponseDto?)> GetByIdAsync(int id)
    {
        var equipment = await _db.Equipments.FindAsync(id);
        if (equipment is null) return (false, "Equipment not found.", null);
        return (true, "Success.", ToDto(equipment));
    }

    public async Task<(bool, string, List<EquipmentResponseDto>?)> GetAllAsync()
    {
        var equipments = await _db.Equipments
            .Select(e => ToDto(e))
            .ToListAsync();
        return (true, "Success.", equipments);
    }

    public async Task<(bool, string, List<EquipmentResponseDto>?)> SearchAsync(string query)
    {
        var normalizedQuery = query.ToLower();
        var equipments = await _db.Equipments.ToListAsync();

        var result = equipments
            .Where(e =>
                e.Title.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                e.Description.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                e.Location.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                e.Brand.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                e.Model.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                IsSimilar(e.Title, normalizedQuery) ||
                IsSimilar(e.Brand, normalizedQuery) ||
                IsSimilar(e.Model, normalizedQuery))
            .OrderBy(e => LevenshteinDistance(e.Title.ToLower(), normalizedQuery))
            .Select(e => ToDto(e))
            .ToList();

        return (true, "Success.", result);
    }

    public async Task<(bool, string, EquipmentResponseDto?)> UpdateAsync(int id, UpdateEquipmentDto dto)
    {
        var equipment = await _db.Equipments.FindAsync(id);
        if (equipment is null) return (false, "Equipment not found.", null);

        equipment.Title = dto.Title;
        equipment.Description = dto.Description;
        equipment.Type = dto.Type;
        equipment.Price = dto.Price;
        equipment.Location = dto.Location;
        equipment.ManufacturingDate = dto.ManufacturingDate;
        equipment.ListingType = dto.ListingType;
        equipment.Condition = dto.Condition;
        equipment.Brand = dto.Brand;
        equipment.Model = dto.Model;
        equipment.IsAvailable = dto.IsAvailable;
        equipment.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return (true, "Equipment updated successfully.", ToDto(equipment));
    }

    public async Task<(bool, string)> DeleteAsync(int id)
    {
        var equipment = await _db.Equipments.FindAsync(id);
        if (equipment is null) return (false, "Equipment not found.");

        _db.Equipments.Remove(equipment);
        await _db.SaveChangesAsync();
        return (true, "Equipment deleted successfully.");
    }

    private static bool IsSimilar(string source, string query, int threshold = 5)
    {
        var words = source.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Any(word =>
        {
            var normalizedWord = word.ToLower();
            return normalizedWord.StartsWith(query) ||
                   normalizedWord.Contains(query) ||
                   LevenshteinDistance(normalizedWord, query) <= threshold;
        });
    }

    private static int LevenshteinDistance(string a, string b)
    {
        int[,] matrix = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) matrix[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) matrix[0, j] = j;
        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        return matrix[a.Length, b.Length];
    }

    private static EquipmentResponseDto ToDto(Equipment e) => new()
    {
        Id = e.Id,
        Title = e.Title,
        Description = e.Description,
        Type = e.Type,
        Price = e.Price,
        Location = e.Location,
        ManufacturingDate = e.ManufacturingDate,
        ListingType = e.ListingType,
        Condition = e.Condition,
        Brand = e.Brand,
        Model = e.Model,
        IsAvailable = e.IsAvailable,
        CreatedAt = e.CreatedAt
    };
}