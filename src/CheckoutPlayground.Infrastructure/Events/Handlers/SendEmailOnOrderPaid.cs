using CheckoutPlayground.Domain.Common;
using CheckoutPlayground.Domain.Orders.Events;
using CheckoutPlayground.Infrastructure.Events;

namespace CheckoutPlayground.Infrastructure.Events.Handlers;

public sealed class SendEmailOnOrderPaid : IEventHandler
{
    public Task TryHandleAsync(DomainEvent ev, CancellationToken ct)
    {
        if (ev is not OrderPaidEvent paid) return Task.CompletedTask;

        Console.WriteLine($"[Email] Order {paid.OrderId} paid: {paid.PaidAmount}");
        return Task.CompletedTask;
    }
}