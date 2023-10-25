namespace NotionBackend.Api.Domain.DTOs.Auth;

using System.Text.Json;
using System.ComponentModel.DataAnnotations;

public class DoLoginDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}