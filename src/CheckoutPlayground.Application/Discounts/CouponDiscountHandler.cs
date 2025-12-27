using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Application.Discounts;

public sealed class CouponDiscountHandler : IDiscountHandler
{
    private IDiscountHandler? _next;

    public IDiscountHandler SetNext(IDiscountHandler next) { _next = next; return next; }

    public Money Handle(DiscountContext context)
    {
        var discount = Money.Zero(context.OrderTotal.Currency);

        if (!string.IsNullOrWhiteSpace(context.CouponCode) && context.CouponCode == "SAVE10")
        {
            discount = new Money(context.OrderTotal.Amount * 0.10m, context.OrderTotal.Currency);
        }

        return discount.Add(_next?.Handle(context) ?? Money.Zero(context.OrderTotal.Currency));
    }
}