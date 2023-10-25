namespace NotionBackend.Api.Application.Services.Jwt;

using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using NotionBackend.Api.Domain.Entities;
using NotionBackend.Api.Domain.DTOs.Auth;

public class JwtService : IJwtService
{
    private readonly ILogger<JwtService> _logger;

    private readonly IConfiguration _configuration;

    public JwtService(ILogger<JwtService> logger, IConfiguration configuration)
    {
        this._logger = logger;
        this._configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(this._configuration["Application:Secret"]);
        var tokenConfig = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", user.Id.ToString()),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Email", user.Email),
            }),
            Expires = DateTime.UtcNow.AddHours(12),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenConfig);

        return tokenHandler.WriteToken(token);
    }
}