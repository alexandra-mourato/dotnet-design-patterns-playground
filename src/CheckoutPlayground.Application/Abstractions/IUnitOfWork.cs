namespace CheckoutPlayground.Application.Abstractions;

public interface IUnitOfWork
{
    Task CommitAsync(CancellationToken ct);
}