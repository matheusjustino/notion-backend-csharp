namespace NotionBackend.Api.WebUI.Controllers;

using Microsoft.AspNetCore.Mvc;
using NotionBackend.Api.Domain.DTOs.Auth;
using NotionBackend.Api.Domain.DTOs.User;
using NotionBackend.Api.Application.Services.Authentication;

[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        this._authenticationService = authenticationService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register([FromBody] RegisterDTO body)
    {
        var newUser = await this._authenticationService.Register(body);
        return Ok(newUser);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> DoLogin([FromBody] DoLoginDTO body)
    {
        var token = await this._authenticationService.DoLogin(body);
        return Ok(token);
    }
}