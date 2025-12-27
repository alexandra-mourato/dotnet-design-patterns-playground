using CheckoutPlayground.Application.Abstractions;
using CheckoutPlayground.Domain.Common;

namespace CheckoutPlayground.Infrastructure.Events;

public sealed class InMemoryEventDispatcher : IEventDispatcher
{
    private readonly IEnumerable<IEventHandler> _handlers;

    public InMemoryEventDispatcher(IEnumerable<IEventHandler> handlers)
        => _handlers = handlers;

    public async Task DispatchAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken ct)
    {
        foreach (var ev in events)
        {
            foreach (var handler in _handlers)
                await handler.TryHandleAsync(ev, ct);
        }
    }
}

public interface IEventHandler
{
    Task TryHandleAsync(DomainEvent ev, CancellationToken ct);
}