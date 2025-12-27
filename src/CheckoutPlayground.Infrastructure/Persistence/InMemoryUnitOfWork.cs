using CheckoutPlayground.Application.Abstractions;

namespace CheckoutPlayground.Infrastructure.Persistence;

public sealed class InMemoryUnitOfWork : IUnitOfWork
{
    public Task CommitAsync(CancellationToken ct) => Task.CompletedTask;
}