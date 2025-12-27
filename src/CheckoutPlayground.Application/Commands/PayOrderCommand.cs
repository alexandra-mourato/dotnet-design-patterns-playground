namespace CheckoutPlayground.Application.Commands;

public sealed record PayOrderCommand(Guid OrderId, string CountryCode, string CustomerTier, string? CouponCode);