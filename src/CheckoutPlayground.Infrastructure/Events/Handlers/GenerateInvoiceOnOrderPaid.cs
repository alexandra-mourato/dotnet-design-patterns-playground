using CheckoutPlayground.Domain.Common;
using CheckoutPlayground.Domain.Orders.Events;
using CheckoutPlayground.Infrastructure.Events;

namespace CheckoutPlayground.Infrastructure.Events.Handlers;

public sealed class GenerateInvoiceOnOrderPaid : IEventHandler
{
    public Task TryHandleAsync(DomainEvent ev, CancellationToken ct)
    {
        if (ev is not OrderPaidEvent paid) return Task.CompletedTask;

        Console.WriteLine($"[Invoice] Generating invoice for order {paid.OrderId}");
        return Task.CompletedTask;
    }
}