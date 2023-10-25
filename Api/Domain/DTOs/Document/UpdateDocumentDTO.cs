namespace NotionBackend.Api.Domain.DTOs.Document;

using System.Text.Json;

public class UpdateDocumentDTO
{
    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? CoverImage { get; set; }

    public string? Icon { get; set; }

    public bool? IsPublished { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}