using Microsoft.EntityFrameworkCore;

public interface IProduceService
{
    Task<(bool Success, string Message, ProduceResponseDto? Data)> CreateAsync(int sellerId, CreateProduceDto dto);
    Task<(bool Success, string Message, ProduceResponseDto? Data)> GetByIdAsync(int id);
    Task<(bool Success, string Message, List<ProduceResponseDto>? Data)> GetAllAsync();
    Task<(bool Success, string Message, List<ProduceResponseDto>? Data)> SearchAsync(string query);
    Task<(bool Success, string Message, ProduceResponseDto? Data)> UpdateAsync(int id, UpdateProduceDto dto);
    Task<(bool Success, string Message)> DeleteAsync(int id);
}

public class ProduceService : IProduceService
{
    private readonly AppDbContext _db;

    public ProduceService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool, string, ProduceResponseDto?)> CreateAsync(int sellerId, CreateProduceDto dto)
    {
        var produce = new Produce
        {
            SellerId = sellerId,                        // NEW
            Title = dto.Title,
            Description = dto.Description,
            Category = dto.Category,
            Price = dto.Price,
            Unit = dto.Unit,
            AvailableUnitsLeft = dto.AvailableUnitsLeft,
            InitialUnitsLeft = dto.AvailableUnitsLeft,  // NEW
            Location = dto.Location,
            HarvestDate = dto.HarvestDate,
            ExpiryDate = dto.ExpiryDate
        };

        _db.Produces.Add(produce);
        await _db.SaveChangesAsync();
        return (true, "Produce listed successfully.", ToDto(produce));
    }

    public async Task<(bool, string, ProduceResponseDto?)> GetByIdAsync(int id)
    {
        var produce = await _db.Produces.FindAsync(id);
        if (produce is null) return (false, "Produce not found.", null);
        return (true, "Success.", ToDto(produce));
    }

    public async Task<(bool, string, List<ProduceResponseDto>?)> GetAllAsync()
    {
        var produces = await _db.Produces
            .Select(p => ToDto(p))
            .ToListAsync();
        return (true, "Success.", produces);
    }

    public async Task<(bool, string, List<ProduceResponseDto>?)> SearchAsync(string query)
    {
        var normalizedQuery = query.ToLower();
        var produces = await _db.Produces.ToListAsync();

        var result = produces
            .Where(p =>
                p.Title.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                p.Location.Contains(normalizedQuery, StringComparison.OrdinalIgnoreCase) ||
                IsSimilar(p.Title, normalizedQuery) ||
                IsSimilar(p.Location, normalizedQuery))
            .OrderBy(p => LevenshteinDistance(p.Title.ToLower(), normalizedQuery))
            .Select(p => ToDto(p))
            .ToList();

        return (true, "Success.", result);
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

    public async Task<(bool, string, ProduceResponseDto?)> UpdateAsync(int id, UpdateProduceDto dto)
    {
        var produce = await _db.Produces.FindAsync(id);
        if (produce is null) return (false, "Produce not found.", null);

        produce.Title = dto.Title;
        produce.Description = dto.Description;
        produce.Category = dto.Category;
        produce.Price = dto.Price;
        produce.Unit = dto.Unit;
        produce.AvailableUnitsLeft = dto.AvailableUnitsLeft;
        // InitialUnitsLeft never updated — permanent record
        produce.Location = dto.Location;
        produce.HarvestDate = dto.HarvestDate;
        produce.ExpiryDate = dto.ExpiryDate;
        produce.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return (true, "Produce updated successfully.", ToDto(produce));
    }

    public async Task<(bool, string)> DeleteAsync(int id)
    {
        var produce = await _db.Produces.FindAsync(id);
        if (produce is null) return (false, "Produce not found.");

        _db.Produces.Remove(produce);
        await _db.SaveChangesAsync();
        return (true, "Produce deleted successfully.");
    }

    private static ProduceResponseDto ToDto(Produce p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        Category = p.Category,
        Price = p.Price,
        Unit = p.Unit,
        AvailableUnitsLeft = p.AvailableUnitsLeft,
        InitialUnitsLeft = p.InitialUnitsLeft,          // NEW
        Location = p.Location,
        HarvestDate = p.HarvestDate,
        ExpiryDate = p.ExpiryDate,
        CreatedAt = p.CreatedAt
    };
}