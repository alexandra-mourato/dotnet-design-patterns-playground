using CheckoutPlayground.Domain.Orders;

namespace CheckoutPlayground.Application.Discounts;

public interface IDiscountHandler
{
    IDiscountHandler SetNext(IDiscountHandler next);
    Money Handle(DiscountContext context);
}