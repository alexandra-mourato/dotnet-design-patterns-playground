namespace CheckoutPlayground.Application.Abstractions.Security;

public interface IJwtTokenService
{
    public string GenerateToken(string username, string role);
}