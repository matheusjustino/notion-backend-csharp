namespace NotionBackend.Api.Domain.DTOs.Document;

using System.Text.Json;
using NotionBackend.Api.Domain.DTOs.User;

public class DocumentDTO
{
    public Guid Id { get; set; }

    public string Title { get; set; }

    public Guid AuthorId { get; set; }

    public UserDTO Author { get; set; }

    public Guid? ParentDocumentId { get; set; }

    public DocumentDTO? ParentDocument { get; set; }

    public string? Content { get; set; }

    public string? CoverImage { get; set; }

    public string? Icon { get; set; }

    public bool? IsPublished { get; set; }

    public bool? IsArchived { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}