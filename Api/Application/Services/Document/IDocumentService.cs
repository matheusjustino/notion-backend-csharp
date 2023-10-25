namespace NotionBackend.Api.Application.Services.Document;

using NotionBackend.Api.Domain.DTOs.Document;

public interface IDocumentService
{
    Task<DocumentDTO> CreateDocument(Guid authorId, CreateDocumentDTO data);

    Task<DocumentDTO?> GetDocument(Guid? authorId, Guid documentId);

    Task<DocumentDTO?> GetDocumentPreview(Guid documentId);

    List<DocumentDTO> ListDocuments(Guid authorId, Guid? parentDocumentId);

    Task<DocumentDTO> UpdateDocument(Guid authorId, Guid documentId, UpdateDocumentDTO data);

    Task<DocumentDTO> RemoveDocumentIcon(Guid authorId, Guid documentId);

    Task<DocumentDTO> RemoveDocumentCoverImage(Guid authorId, Guid documentId);

    Task<DocumentDTO> ArchiveDocument(Guid documentId, Guid authorId);

    Task<List<DocumentDTO>> ListArchiveDocuments(Guid authorId);

    Task<DocumentDTO> RestoreDocument(Guid authorId, Guid documentId);

    Task<DocumentDTO> DeleteDocument(Guid authorId, Guid documentId);
}