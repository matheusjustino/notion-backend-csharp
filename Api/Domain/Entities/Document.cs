namespace NotionBackend.Api.Domain.Entities;

using System.ComponentModel.DataAnnotations;

public class Document : BaseEntity
{
    [Required]
    public string Title { get; set; }

    [Required]
    public Guid AuthorId { get; set; }

    [Required]
    public User Author { get; set; }

    public Guid? ParentDocumentId { get; set; }

    public Document? ParentDocument { get; set; }

    public string? Content { get; set; }

    public string? CoverImage { get; set; }

    public string? Icon { get; set; }

    public bool? IsPublished { get; set; }

    public bool? IsArchived { get; set; }
}