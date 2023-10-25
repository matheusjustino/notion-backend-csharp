namespace NotionBackend.Api.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class User : BaseEntity
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public string? Picture { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    public ICollection<Document> Documents { get; set; } = new List<Document>();
}