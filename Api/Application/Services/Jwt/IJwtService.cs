namespace NotionBackend.Api.Application.Services.Jwt;

using NotionBackend.Api.Domain.Entities;

public interface IJwtService
{
    string GenerateToken(User user);
}