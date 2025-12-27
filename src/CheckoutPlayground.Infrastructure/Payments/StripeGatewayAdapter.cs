using CheckoutPlayground.Application.Payments;

namespace CheckoutPlayground.Infrastructure.Payments;

public sealed class StripeGatewayAdapter : IPaymentGateway
{
    public string Name => "Stripe";

    public Task ChargeAsync(PaymentRequest request, CancellationToken ct)
    {
        Console.WriteLine($"[Stripe] Charging {request.Amount} for order {request.OrderId} ({request.CountryCode})");
        return Task.CompletedTask;
    }
}