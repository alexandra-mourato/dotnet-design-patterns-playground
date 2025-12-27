using CheckoutPlayground.Domain.Common;

namespace CheckoutPlayground.Application.Abstractions;

public interface IEventDispatcher
{
    Task DispatchAsync(IReadOnlyCollection<DomainEvent> events, CancellationToken ct);
}