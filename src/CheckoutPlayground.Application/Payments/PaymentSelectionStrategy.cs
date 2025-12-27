namespace CheckoutPlayground.Application.Payments;

public sealed class PaymentSelectionStrategy
{
    private readonly IReadOnlyCollection<IPaymentGateway> _gateways;

    public PaymentSelectionStrategy(IEnumerable<IPaymentGateway> gateways)
        => _gateways = gateways.ToList();

    public IPaymentGateway Select(string countryCode)
    {
        // Demo rule: PT -> Stripe, others -> PayPal
        var preferred = countryCode == "PT" ? "Stripe" : "PayPal";

        return _gateways.FirstOrDefault(g => g.Name == preferred)
               ?? _gateways.First();
    }
}