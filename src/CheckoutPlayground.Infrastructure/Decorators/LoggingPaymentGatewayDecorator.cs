using CheckoutPlayground.Application.Payments;

namespace CheckoutPlayground.Infrastructure.Decorators;

public sealed class LoggingPaymentGatewayDecorator : IPaymentGateway
{
    private readonly IPaymentGateway _inner;
    public string Name => _inner.Name;

    public LoggingPaymentGatewayDecorator(IPaymentGateway inner) => _inner = inner;

    public async Task ChargeAsync(PaymentRequest request, CancellationToken ct)
    {
        Console.WriteLine($"[Log] Starting charge via {Name}...");
        await _inner.ChargeAsync(request, ct);
        Console.WriteLine($"[Log] Charge finished via {Name}.");
    }
}