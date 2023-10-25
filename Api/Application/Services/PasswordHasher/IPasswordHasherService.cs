namespace NotionBackend.Api.Application.Services.PasswordHasher;

public interface IPasswordHasherService
{
    string Hash(string password);

    bool Verify(string passwordHash, string inputPassword);
}
