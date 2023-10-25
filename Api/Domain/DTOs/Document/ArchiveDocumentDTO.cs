namespace NotionBackend.Api.Domain.DTOs.Document;

using System.ComponentModel.DataAnnotations;

public class ArchiveDocumentDTO
{
    [Required]
    public Guid DocumentId { get; set; }

    public Guid? AuthorId { get; set; }
}