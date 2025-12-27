using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Application.Discounts;

public sealed record DiscountContext(string CountryCode, string CustomerTier, string? CouponCode, Money OrderTotal);