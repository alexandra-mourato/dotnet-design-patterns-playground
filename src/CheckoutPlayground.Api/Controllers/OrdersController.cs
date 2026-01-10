using CheckoutPlayground.Application.Abstractions;
using CheckoutPlayground.Domain.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/orders")]
[Authorize]
public sealed class OrdersController : ControllerBase
{
    private readonly ICheckoutFacade _checkout;

    public OrdersController(ICheckoutFacade checkout)
    {
        _checkout = checkout;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOrderRequest req, CancellationToken ct)
    {
        var id = await _checkout.CreateOrderAsync(
            new Money(req.TotalAmount, req.Currency), ct);

        return Ok(new { orderId = id });
    }

    [HttpPost("{orderId:guid}/pay")]
    public async Task<IActionResult> Pay(
        Guid orderId,
        PayOrderRequest req,
        CancellationToken ct)
    {
        await _checkout.PayOrderAsync(
            orderId, req.CountryCode, req.CustomerTier, req.CouponCode, ct);

        return Ok(new { orderId, status = "Paid" });
    }
}

public sealed record CreateOrderRequest(decimal TotalAmount, string Currency);
public sealed record PayOrderRequest(string CountryCode, string CustomerTier, string? CouponCode);