namespace NotionBackend.Api.Domain.DTOs.User;

using System.Text.Json;
using NotionBackend.Api.Domain.DTOs.Document;

public class UserDTO
{
    public Guid Id { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public string? Picture { get; set; }

    public List<DocumentDTO> Documents { get; set; } = new ();

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}