using CheckoutPlayground.Application.Abstractions;
using CheckoutPlayground.Application.Commands;
using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Application.Checkout;

public sealed class CheckoutFacade : ICheckoutFacade
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly PayOrderHandler _payHandler;

    public CheckoutFacade(IOrderRepository orders, IUnitOfWork uow, PayOrderHandler payHandler)
    {
        _orders = orders;
        _uow = uow;
        _payHandler = payHandler;
    }

    public async Task<Guid> CreateOrderAsync(Money total, CancellationToken ct)
    {
        var order = new Order(total);
        await _orders.AddAsync(order, ct);
        await _uow.CommitAsync(ct);
        return order.Id;
    }

    public Task PayOrderAsync(Guid orderId, string countryCode, string customerTier, string? couponCode, CancellationToken ct)
        => _payHandler.HandleAsync(new PayOrderCommand(orderId, countryCode, customerTier, couponCode), ct);
}