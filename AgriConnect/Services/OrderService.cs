using Microsoft.EntityFrameworkCore;

public interface IOrderService
{
    Task<(bool Success, string Message, OrderResponseDto? Data)> CreateAsync(int userId, CreateOrderDto dto);
    Task<(bool Success, string Message, List<OrderResponseDto>? Data)> GetMyOrdersAsync(int userId);
    Task<(bool Success, string Message, OrderResponseDto? Data)> UpdateQuantityAsync(int userId, int orderId, UpdateOrderDto dto);
    Task<(bool Success, string Message)> CancelAsync(int userId, int orderId);
}

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<(bool, string, OrderResponseDto?)> CreateAsync(int userId, CreateOrderDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            if (dto.ItemType == ItemType.Produce)
            {
                var produce = await _db.Produces
                    .FromSqlRaw("SELECT * FROM \"Produces\" WHERE \"Id\" = {0} FOR UPDATE", dto.ItemId)
                    .FirstOrDefaultAsync();

                if (produce is null)
                    return (false, "Produce item not found.", null);

                if (produce.ExpiryDate <= DateTime.UtcNow)
                    return (false, $"This produce expired on {produce.ExpiryDate:yyyy-MM-dd} and cannot be ordered.", null);

                if (produce.AvailableUnitsLeft <= 0)
                    return (false, "This produce is out of stock.", null);

                if (dto.Quantity > produce.AvailableUnitsLeft)
                    return (false, $"Insufficient stock. Only {produce.AvailableUnitsLeft} {produce.Unit} available. Please lower your order quantity.", null);

                var total = (decimal)dto.Quantity * produce.Price;

                var order = new Order
                {
                    UserId = userId,
                    SellerId = produce.SellerId,
                    ItemType = ItemType.Produce,
                    ProduceId = produce.Id,
                    Quantity = dto.Quantity,
                    PriceSnapshot = produce.Price,
                    TotalAmount = total,
                    Status = OrderStatus.Active
                };

                produce.AvailableUnitsLeft -= dto.Quantity;
                produce.UpdatedAt = DateTime.UtcNow;

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                _db.Incomes.Add(new Income
                {
                    OrderId = order.Id,
                    Amount = Math.Round(total * 0.015m, 2)
                });
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "Order placed successfully.", ToDto(order, produce.Title));
            }
            else
            {
                var equipment = await _db.Equipments
                    .FromSqlRaw("SELECT * FROM \"Equipments\" WHERE \"Id\" = {0} FOR UPDATE", dto.ItemId)
                    .FirstOrDefaultAsync();

                if (equipment is null)
                    return (false, "Equipment not found.", null);

                if (!equipment.IsAvailable)
                    return (false, "This equipment is currently unavailable.", null);

                var total = equipment.Price;

                var order = new Order
                {
                    UserId = userId,
                    SellerId = equipment.SellerId,
                    ItemType = ItemType.Equipment,
                    EquipmentId = equipment.Id,
                    Quantity = 1,
                    PriceSnapshot = equipment.Price,
                    TotalAmount = total,
                    ListingType = equipment.ListingType,
                    Status = OrderStatus.Active
                };

                equipment.IsAvailable = false;
                equipment.UpdatedAt = DateTime.UtcNow;

                _db.Orders.Add(order);
                await _db.SaveChangesAsync();

                _db.Incomes.Add(new Income
                {
                    OrderId = order.Id,
                    Amount = Math.Round(total * 0.015m, 2)
                });
                await _db.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "Order placed successfully.", ToDto(order, equipment.Title));
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Order failed: {ex.Message}", null);
        }
    }

    public async Task<(bool, string, List<OrderResponseDto>?)> GetMyOrdersAsync(int userId)
    {
        var orders = await _db.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Produce)
            .Include(o => o.Equipment)
            .ToListAsync();

        var result = orders
            .Select(o => ToDto(o,
                o.ItemType == ItemType.Produce
                    ? o.Produce?.Title ?? ""
                    : o.Equipment?.Title ?? ""))
            .ToList();

        return (true, "Success.", result);
    }

    public async Task<(bool, string, OrderResponseDto?)> UpdateQuantityAsync(int userId, int orderId, UpdateOrderDto dto)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var order = await _db.Orders.FindAsync(orderId);

            if (order is null || order.UserId != userId)
                return (false, "Order not found.", null);

            if (order.Status == OrderStatus.Cancelled)
                return (false, "Cannot modify a cancelled order.", null);

            if (order.ItemType != ItemType.Produce)
                return (false, "Quantity update is only applicable for produce orders.", null);

            var produce = await _db.Produces
                .FromSqlRaw("SELECT * FROM \"Produces\" WHERE \"Id\" = {0} FOR UPDATE", order.ProduceId!)
                .FirstOrDefaultAsync();

            if (produce is null)
                return (false, "Produce item no longer exists.", null);

            if (produce.ExpiryDate <= DateTime.UtcNow)
                return (false, "This produce has expired and the order cannot be modified.", null);

            var quantityDiff = dto.Quantity - order.Quantity;

            // Increasing quantity — check available stock
            if (quantityDiff > 0 && quantityDiff > produce.AvailableUnitsLeft)
                return (false, $"Cannot increase quantity. Only {produce.AvailableUnitsLeft} {produce.Unit} additional stock available.", null);

            // Restore or deduct based on diff
            var newStock = produce.AvailableUnitsLeft - quantityDiff;

            if (newStock < 0)
            {
                return (false, "Stock cannot go negative.", null);
            }

            produce.AvailableUnitsLeft = newStock;
            produce.UpdatedAt = DateTime.UtcNow;

            var newTotal = (decimal)dto.Quantity * order.PriceSnapshot;

            var income = await _db.Incomes.FirstOrDefaultAsync(i => i.OrderId == order.Id);
            if (income is not null)
                income.Amount = Math.Round(newTotal * 0.015m, 2);

            order.Quantity = dto.Quantity;
            order.TotalAmount = newTotal;
            order.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "Order quantity updated successfully.", ToDto(order, produce.Title));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Update failed: {ex.Message}", null);
        }
    }

    public async Task<(bool, string)> CancelAsync(int userId, int orderId)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();

        try
        {
            var order = await _db.Orders.FindAsync(orderId);

            if (order is null || order.UserId != userId)
                return (false, "Order not found.");

            if (order.Status == OrderStatus.Cancelled)
                return (false, "Order is already cancelled.");

            order.Status = OrderStatus.Cancelled;
            order.UpdatedAt = DateTime.UtcNow;

            if (order.ItemType == ItemType.Produce && order.ProduceId.HasValue)
            {
                var produce = await _db.Produces.FindAsync(order.ProduceId.Value);
                if (produce is not null)
                {
                    produce.AvailableUnitsLeft += order.Quantity;
                    produce.UpdatedAt = DateTime.UtcNow;
                }
            }
            else if (order.ItemType == ItemType.Equipment && order.EquipmentId.HasValue)
            {
                var equipment = await _db.Equipments.FindAsync(order.EquipmentId.Value);
                if (equipment is not null)
                {
                    equipment.IsAvailable = true;
                    equipment.UpdatedAt = DateTime.UtcNow;
                }
            }

            var income = await _db.Incomes.FirstOrDefaultAsync(i => i.OrderId == order.Id);
            if (income is not null)
                _db.Incomes.Remove(income);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return (true, "Order cancelled successfully. Stock has been restored.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return (false, $"Cancellation failed: {ex.Message}");
        }
    }

    private static OrderResponseDto ToDto(Order o, string itemTitle) => new()
    {
        Id = o.Id,
        UserId = o.UserId,
        ItemType = o.ItemType,
        ProduceId = o.ProduceId,
        EquipmentId = o.EquipmentId,
        ItemTitle = itemTitle,
        Quantity = o.Quantity,
        PriceSnapshot = o.PriceSnapshot,
        TotalAmount = o.TotalAmount,
        ListingType = o.ListingType,
        Status = o.Status,
        CreatedAt = o.CreatedAt
    };
}