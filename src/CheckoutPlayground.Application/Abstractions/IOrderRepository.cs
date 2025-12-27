using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Application.Abstractions;

public interface IOrderRepository
{
    Task<Order?> GetAsync(Guid id, CancellationToken ct);
    Task AddAsync(Order order, CancellationToken ct);
    Task SaveAsync(Order order, CancellationToken ct);
}