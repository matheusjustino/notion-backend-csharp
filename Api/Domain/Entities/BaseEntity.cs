namespace NotionBackend.Api.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; init; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; }
}