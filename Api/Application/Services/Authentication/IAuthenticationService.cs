namespace NotionBackend.Api.Application.Services.Authentication;

using NotionBackend.Api.Domain.DTOs.Auth;
using NotionBackend.Api.Domain.DTOs.User;

public interface IAuthenticationService
{
    Task<UserDTO> Register(RegisterDTO data);

    Task<string> DoLogin(DoLoginDTO data);
}