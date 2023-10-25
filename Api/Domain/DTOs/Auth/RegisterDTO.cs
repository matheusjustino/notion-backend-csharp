namespace NotionBackend.Api.Domain.DTOs.Auth;

using System.Text.Json;
using System.ComponentModel.DataAnnotations;

public class RegisterDTO
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public string? Picture { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}