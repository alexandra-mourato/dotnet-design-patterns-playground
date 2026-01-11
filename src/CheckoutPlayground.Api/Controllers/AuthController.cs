using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CheckoutPlayground.Application.Abstractions.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CheckoutPlayground.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    
    private readonly IJwtTokenService _jwt;

    public AuthController(IJwtTokenService jwt)
    {
        _jwt = jwt;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public IActionResult GenerateToken([FromBody] LoginRequest req)
    {
        // DEMO ONLY
        if (req.Username != "admin" || req.Password != "admin")
            return Unauthorized();

        var token = _jwt.GenerateToken(req.Username, "Admin");
        
        return Ok(new
        {
            access_token = token,
            token_type = "Bearer"
        }); 
    }
    
    public sealed record LoginRequest(string Username, string Password);
}
