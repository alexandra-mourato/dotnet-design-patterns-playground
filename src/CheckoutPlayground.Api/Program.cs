using System.Text;
using CheckoutPlayground.Application.Abstractions;
using CheckoutPlayground.Application.Checkout;
using CheckoutPlayground.Application.Commands;
using CheckoutPlayground.Application.Discounts;
using CheckoutPlayground.Application.Payments;
using CheckoutPlayground.Infrastructure.Decorators;
using CheckoutPlayground.Infrastructure.Events;
using CheckoutPlayground.Infrastructure.Events.Handlers;
using CheckoutPlayground.Infrastructure.Payments;
using CheckoutPlayground.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

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
builder.Services.AddSingleton<IDiscountHandler>(_ =>
{
    var coupon = new CouponDiscountHandler();
    var loyalty = new LoyaltyDiscountHandler();
    var seasonal = new SeasonalDiscountHandler();

    coupon.SetNext(loyalty).SetNext(seasonal);
    return coupon;
});

// Payments (Adapters + Decorators + Strategy)
// ⚠️ Nota: duas registrations do mesmo service substituem/competem. Mantive como estava.
builder.Services.AddSingleton<IPaymentGateway>(_ =>
    new RetryPaymentGatewayDecorator(
        new LoggingPaymentGatewayDecorator(new StripeGatewayAdapter())
    ));

builder.Services.AddSingleton<IPaymentGateway>(_ =>
    new RetryPaymentGatewayDecorator(
        new LoggingPaymentGatewayDecorator(new PaypalGatewayAdapter())
    ));

builder.Services.AddSingleton<PaymentSelectionStrategy>();

// Commands + Facade
builder.Services.AddSingleton<PayOrderHandler>();
builder.Services.AddSingleton<ICheckoutFacade, CheckoutFacade>();

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Checkout Playground API", 
            Version = "v1" 
            
        }
    );

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}"
    });
});

// Auth
var jwtSection = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSection["Key"]!;
var jwtIssuer = jwtSection["Issuer"]!;
var jwtAudience = jwtSection["Audience"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = jwtAudience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Checkout Playground API v1");
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();