using CheckoutPlayground.Domain.Common;
using CheckoutPlayground.Domain.Orders.Events;

namespace CheckoutPlayground.Domain.Orders;

public sealed class Order : Entity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Money Total { get; private set; }
    public Money DiscountTotal { get; private set; }
    public OrderStatus Status => _state.Status;

    private IOrderState _state = new CreatedState();

    public Order(Money total)
    {
        Total = total;
        DiscountTotal = Money.Zero(total.Currency);
    }

    public void ApplyDiscount(Money discount)
    {
        if (Status != OrderStatus.Created)
            throw new InvalidOperationException("Discounts can only be applied to Created orders.");

        DiscountTotal = DiscountTotal.Add(discount);
    }

    public Money AmountToPay()
    {
        var payable = Total.Subtract(DiscountTotal);
        if (payable.Amount < 0m) return Money.Zero(Total.Currency);
        return payable;
    }

    public void Pay(Money paidAmount) => _state.Pay(this, paidAmount);
    public void Cancel() => _state.Cancel(this);

    internal void TransitionTo(IOrderState newState, Money? paidAmount)
    {
        _state = newState;

        if (newState.Status == OrderStatus.Paid && paidAmount is not null)
            Raise(new OrderPaidEvent(Id, paidAmount.Value));
    }
}