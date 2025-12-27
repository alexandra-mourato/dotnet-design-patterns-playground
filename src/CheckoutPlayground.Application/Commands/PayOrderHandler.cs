using CheckoutPlayground.Application.Abstractions;
using CheckoutPlayground.Application.Discounts;
using CheckoutPlayground.Application.Payments;

namespace CheckoutPlayground.Application.Commands;

public sealed class PayOrderHandler
{
    private readonly IOrderRepository _orders;
    private readonly IUnitOfWork _uow;
    private readonly IEventDispatcher _events;
    private readonly PaymentSelectionStrategy _paymentSelection;
    private readonly IDiscountHandler _discountChain;

    public PayOrderHandler(
        IOrderRepository orders,
        IUnitOfWork uow,
        IEventDispatcher events,
        PaymentSelectionStrategy paymentSelection,
        IDiscountHandler discountChain)
    {
        _orders = orders;
        _uow = uow;
        _events = events;
        _paymentSelection = paymentSelection;
        _discountChain = discountChain;
    }

    public async Task HandleAsync(PayOrderCommand cmd, CancellationToken ct)
    {
        var order = await _orders.GetAsync(cmd.OrderId, ct)
                    ?? throw new InvalidOperationException("Order not found.");

        var ctx = new DiscountContext(cmd.CountryCode, cmd.CustomerTier, cmd.CouponCode, order.Total);
        var discount = _discountChain.Handle(ctx);
        order.ApplyDiscount(discount);

        var payable = order.AmountToPay();

        var gateway = _paymentSelection.Select(cmd.CountryCode);
        await gateway.ChargeAsync(new PaymentRequest(order.Id, payable, cmd.CountryCode), ct);

        order.Pay(payable);

        await _orders.SaveAsync(order, ct);
        await _uow.CommitAsync(ct);

        await _events.DispatchAsync(order.DomainEvents, ct);
        order.ClearDomainEvents();
    }
}