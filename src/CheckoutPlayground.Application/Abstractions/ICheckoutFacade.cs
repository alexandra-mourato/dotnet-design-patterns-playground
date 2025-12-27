using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Application.Abstractions;

public interface ICheckoutFacade
{
    Task<Guid> CreateOrderAsync(Money total, CancellationToken ct);
    Task PayOrderAsync(Guid orderId, string countryCode, string customerTier, string? couponCode, CancellationToken ct);
}