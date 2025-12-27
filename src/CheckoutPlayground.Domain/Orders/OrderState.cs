namespace CheckoutPlayground.Domain.Orders;

public enum OrderStatus
{
    Created = 0,
    Paid = 1,
    Cancelled = 2
}

public interface IOrderState
{
    OrderStatus Status { get; }
    void Pay(Order order, Money paidAmount);
    void Cancel(Order order);
}

public sealed class CreatedState : IOrderState
{
    public OrderStatus Status => OrderStatus.Created;

    public void Pay(Order order, Money paidAmount) => order.TransitionTo(new PaidState(), paidAmount);

    public void Cancel(Order order) => order.TransitionTo(new CancelledState(), null);
}

public sealed class PaidState : IOrderState
{
    public OrderStatus Status => OrderStatus.Paid;

    public void Pay(Order order, Money paidAmount) =>
        throw new InvalidOperationException("Order is already paid.");

    public void Cancel(Order order) =>
        throw new InvalidOperationException("Paid orders cannot be cancelled in this demo.");
}

public sealed class CancelledState : IOrderState
{
    public OrderStatus Status => OrderStatus.Cancelled;

    public void Pay(Order order, Money paidAmount) =>
        throw new InvalidOperationException("Cancelled orders cannot be paid.");

    public void Cancel(Order order) =>
        throw new InvalidOperationException("Order is already cancelled.");
}