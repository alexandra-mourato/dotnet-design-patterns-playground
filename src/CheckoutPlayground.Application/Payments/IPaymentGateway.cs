namespace CheckoutPlayground.Application.Payments;

public interface IPaymentGateway
{
    string Name { get; }
    Task ChargeAsync(PaymentRequest request, CancellationToken ct);
}