using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Application.Payments;

public sealed record PaymentRequest(Guid OrderId, Money Amount, string CountryCode);