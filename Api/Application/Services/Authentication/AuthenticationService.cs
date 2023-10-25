namespace NotionBackend.Api.Application.Services.Authentication;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NotionBackend.Api.Domain.DTOs.Auth;
using NotionBackend.Api.Domain.DTOs.User;
using NotionBackend.Api.Domain.Entities;
using NotionBackend.Api.Application.Services.Jwt;
using NotionBackend.Api.Infrastructure.Persistence;
using NotionBackend.Api.Application.Services.PasswordHasher;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILogger<AuthenticationService> _logger;

    private readonly AppDbContext _context;

    private readonly IMapper _mapper;

    private readonly IPasswordHasherService _passwordHasherService;

    private readonly IJwtService _jwtService;

    public AuthenticationService(
        ILogger<AuthenticationService> logger,
        AppDbContext context,
        IMapper mapper,
        IPasswordHasherService passwordHasherService,
        IJwtService jwtService)
    {
        this._logger = logger;
        this._context = context;
        this._mapper = mapper;
        this._passwordHasherService = passwordHasherService;
        this._jwtService = jwtService;
    }

    public async Task<UserDTO> Register(RegisterDTO data)
    {
        this._logger.Log(LogLevel.Information, $"Register - data: ${data}");

        var hashedPassword = this._passwordHasherService.Hash(data.Password);

        var newUser = new User()
        {
            FirstName = data.FirstName,
            LastName = data.LastName,
            Email = data.Email,
            Password = hashedPassword,
        };

        await this._context.Users.AddAsync(newUser);
        await this._context.SaveChangesAsync();

        return this._mapper.Map<UserDTO>(newUser);
    }

    public async Task<string> DoLogin(DoLoginDTO data)
    {
        this._logger.Log(LogLevel.Information, $"Do Login - data: ${data}");

        var user = await this._context.Users.FirstOrDefaultAsync(u => u.Email == data.Email);
        if (user is null)
        {
            throw new BadHttpRequestException("Invalid Credentials");
        }

        var validPassword = this._passwordHasherService.Verify(user.Password, data.Password);
        if (!validPassword)
        {
            throw new BadHttpRequestException("Invalid Credentials");
        }

        return this._jwtService.GenerateToken(user);
    }
}