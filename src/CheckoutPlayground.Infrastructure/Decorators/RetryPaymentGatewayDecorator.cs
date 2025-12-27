using CheckoutPlayground.Application.Payments;

namespace CheckoutPlayground.Infrastructure.Decorators;

public sealed class RetryPaymentGatewayDecorator : IPaymentGateway
{
    private readonly IPaymentGateway _inner;
    private readonly int _maxAttempts;

    public string Name => _inner.Name;

    public RetryPaymentGatewayDecorator(IPaymentGateway inner, int maxAttempts = 3)
    {
        _inner = inner;
        _maxAttempts = Math.Max(1, maxAttempts);
    }

    public async Task ChargeAsync(PaymentRequest request, CancellationToken ct)
    {
        Exception? last = null;

        for (var attempt = 1; attempt <= _maxAttempts; attempt++)
        {
            try
            {
                await _inner.ChargeAsync(request, ct);
                return;
            }
            catch (Exception ex) when (attempt < _maxAttempts)
            {
                last = ex;
                Console.WriteLine($"[Retry] Attempt {attempt} failed via {Name}: {ex.Message}");
            }
        }

        throw new InvalidOperationException($"Charge failed after {_maxAttempts} attempts via {Name}.", last);
    }
}