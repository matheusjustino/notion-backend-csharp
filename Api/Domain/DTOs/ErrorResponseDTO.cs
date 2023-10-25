namespace NotionBackend.Api.Domain.DTOs;

using System.Text.Json;

public class ErrorResponseDTO
{
    public Guid ErrorId { get; set; } = Guid.NewGuid();

    public string Message { get; set; }

    public string Endpoint { get; set; }

    public int StatusCode { get; set; }

    public DateTime Timestamp { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
