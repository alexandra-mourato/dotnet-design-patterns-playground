using CheckoutPlayground.Domain.Common;

namespace CheckoutPlayground.Domain.Orders.Events;

public sealed record OrderPaidEvent(Guid OrderId, Money PaidAmount)
    : DomainEvent(DateTime.UtcNow);