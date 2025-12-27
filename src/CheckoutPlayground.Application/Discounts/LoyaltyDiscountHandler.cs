using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Application.Discounts;

public sealed class LoyaltyDiscountHandler : IDiscountHandler
{
    private IDiscountHandler? _next;

    public IDiscountHandler SetNext(IDiscountHandler next) { _next = next; return next; }

    public Money Handle(DiscountContext context)
    {
        var discount = Money.Zero(context.OrderTotal.Currency);

        if (string.Equals(context.CustomerTier, "Gold", StringComparison.OrdinalIgnoreCase))
        {
            discount = new Money(context.OrderTotal.Amount * 0.05m, context.OrderTotal.Currency);
        }

        return discount.Add(_next?.Handle(context) ?? Money.Zero(context.OrderTotal.Currency));
    }
}