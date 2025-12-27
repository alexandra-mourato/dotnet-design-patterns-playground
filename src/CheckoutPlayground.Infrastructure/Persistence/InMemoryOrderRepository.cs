using CheckoutPlayground.Application.Abstractions;
using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Infrastructure.Persistence;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly Dictionary<Guid, Order> _db = new();

    public Task<Order?> GetAsync(Guid id, CancellationToken ct)
        => Task.FromResult(_db.TryGetValue(id, out var order) ? order : null);

    public Task AddAsync(Order order, CancellationToken ct)
    {
        _db[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task SaveAsync(Order order, CancellationToken ct)
    {
        _db[order.Id] = order;
        return Task.CompletedTask;
    }
}