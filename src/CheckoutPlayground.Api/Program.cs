using CheckoutPlayground.Application.Abstractions;
using CheckoutPlayground.Application.Checkout;
using CheckoutPlayground.Application.Commands;
using CheckoutPlayground.Application.Discounts;
using CheckoutPlayground.Application.Payments;
using CheckoutPlayground.Domain.Orders;
using CheckoutPlayground.Infrastructure.Decorators;
using CheckoutPlayground.Infrastructure.Events;
using CheckoutPlayground.Infrastructure.Events.Handlers;
using CheckoutPlayground.Infrastructure.Payments;
using CheckoutPlayground.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Persistence
builder.Services.AddSingleton<IOrderRepository, InMemoryOrderRepository>();
builder.Services.AddSingleton<IUnitOfWork, InMemoryUnitOfWork>();

// Domain events (Observer)
builder.Services.AddSingleton<IEventHandler, SendEmailOnOrderPaid>();
builder.Services.AddSingleton<IEventHandler, DecreaseStockOnOrderPaid>();
builder.Services.AddSingleton<IEventHandler, GenerateInvoiceOnOrderPaid>();
builder.Services.AddSingleton<IEventDispatcher, InMemoryEventDispatcher>();

// Discounts (Chain of Responsibility)
builder.Services.AddSingleton<IDiscountHandler>(sp =>
{
    var coupon = new CouponDiscountHandler();
    var loyalty = new LoyaltyDiscountHandler();
    var seasonal = new SeasonalDiscountHandler();

    coupon.SetNext(loyalty).SetNext(seasonal);
    return coupon;
});

// Payments (Adapters + Decorators + Strategy)
builder.Services.AddSingleton<IPaymentGateway>(sp =>
    new RetryPaymentGatewayDecorator(
        new LoggingPaymentGatewayDecorator(new StripeGatewayAdapter())
    ));

builder.Services.AddSingleton<IPaymentGateway>(sp =>
    new RetryPaymentGatewayDecorator(
        new LoggingPaymentGatewayDecorator(new PaypalGatewayAdapter())
    ));

builder.Services.AddSingleton<PaymentSelectionStrategy>();

// Commands + Facade
builder.Services.AddSingleton<PayOrderHandler>();
builder.Services.AddSingleton<ICheckoutFacade, CheckoutFacade>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/orders", async (CreateOrderRequest req, ICheckoutFacade checkout, CancellationToken ct) =>
{
    var id = await checkout.CreateOrderAsync(new Money(req.TotalAmount, req.Currency), ct);
    return Results.Ok(new { orderId = id });
});

app.MapPost("/orders/{orderId:guid}/pay", async (Guid orderId, PayOrderRequest req, ICheckoutFacade checkout, CancellationToken ct) =>
{
    await checkout.PayOrderAsync(orderId, req.CountryCode, req.CustomerTier, req.CouponCode, ct);
    return Results.Ok(new { orderId, status = "Paid" });
});

app.Run();

public sealed record CreateOrderRequest(decimal TotalAmount, string Currency);
public sealed record PayOrderRequest(string CountryCode, string CustomerTier, string? CouponCode);
