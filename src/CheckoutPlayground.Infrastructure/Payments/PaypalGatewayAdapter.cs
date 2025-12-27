using CheckoutPlayground.Application.Payments;

namespace CheckoutPlayground.Infrastructure.Payments;

public sealed class PaypalGatewayAdapter : IPaymentGateway
{
    public string Name => "PayPal";

    public Task ChargeAsync(PaymentRequest request, CancellationToken ct)
    {
        Console.WriteLine($"[PayPal] Charging {request.Amount} for order {request.OrderId} ({request.CountryCode})");
        return Task.CompletedTask;
    }
}