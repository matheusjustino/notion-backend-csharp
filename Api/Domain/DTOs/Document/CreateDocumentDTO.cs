namespace NotionBackend.Api.Domain.DTOs.Document;

using System.Text.Json;
using System.ComponentModel.DataAnnotations;

public class CreateDocumentDTO
{
    [Required]
    public string Title { get; set; } = "Untitled";

    public bool? IsArchived { get; set; }

    public Guid? ParentDocument { get; set; }

    public string? Content { get; set; }

    public IFormFile? CoverImage { get; set; }

    public string? Icon { get; set; }

    public bool? IsPublished { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}