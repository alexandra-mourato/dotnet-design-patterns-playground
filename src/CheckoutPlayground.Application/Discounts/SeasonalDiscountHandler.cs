using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Application.Discounts;

public sealed class SeasonalDiscountHandler : IDiscountHandler
{
    private IDiscountHandler? _next;

    public IDiscountHandler SetNext(IDiscountHandler next) { _next = next; return next; }

    public Money Handle(DiscountContext context)
    {
        // Demo: fixed seasonal discount for a specific country
        var discount = context.CountryCode == "PT"
            ? new Money(2.00m, context.OrderTotal.Currency)
            : Money.Zero(context.OrderTotal.Currency);

        return discount.Add(_next?.Handle(context) ?? Money.Zero(context.OrderTotal.Currency));
    }
}