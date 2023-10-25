namespace NotionBackend.Api.Application.Services.Document;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NotionBackend.Api.Domain.DTOs.Document;
using NotionBackend.Api.Domain.Entities;
using NotionBackend.Api.Infrastructure.Persistence;
using NotionBackend.Api.Infrastructure.Configurations;
using NotionBackend.Api.Application.Services.ManageImage;

public class DocumentService : IDocumentService
{
    private readonly ILogger<DocumentService> _logger;

    private readonly AppDbContext _context;

    private readonly IMapper _mapper;

    private readonly IManageImageService _manageImageService;

    public DocumentService(
        ILogger<DocumentService> logger,
        AppDbContext context,
        IManageImageService manageImageService)
    {
        this._logger = logger;
        this._context = context;
        this._manageImageService = manageImageService;

        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new ModelToDTOMapping()));
        var createMapper = mapperConfig.CreateMapper();
        this._mapper = createMapper;
    }

    public async Task<DocumentDTO> CreateDocument(Guid authorId, CreateDocumentDTO data)
    {
        this._logger.LogInformation($"Create Document - data: {data}");

        var author = await this._context.Users.FirstOrDefaultAsync(u => u.Id == authorId);
        if (author is null)
        {
            throw new BadHttpRequestException("Author not found");
        }

        var parentDocument = await this._context.Documents.FirstOrDefaultAsync(d => d.Id == data.ParentDocument);

        await using var transaction = await this._context.Database.BeginTransactionAsync();
        var filename = data.CoverImage != null ? await this._manageImageService.UploadFile(data.CoverImage) : null;

        try
        {
            var newDocument = new Document
            {
                Title = data.Title,
                AuthorId = authorId,
                Author = author,
                ParentDocumentId = data.ParentDocument,
                ParentDocument = parentDocument,
                Content = data.Content,
                CoverImage = filename,
                Icon = data.Icon,
                IsPublished = data.IsPublished,
                IsArchived = data.IsArchived,
            };

            await this._context.AddAsync(newDocument);
            await this._context.SaveChangesAsync();
            await transaction.CommitAsync();

            return this._mapper.Map<DocumentDTO>(newDocument);
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            if (!string.IsNullOrEmpty(filename))
            {
                this._manageImageService.DeleteImage(filename);
            }

            throw;
        }
    }

    public async Task<DocumentDTO?> GetDocument(Guid? authorId, Guid documentId)
    {
        this._logger.LogInformation("Get Document - AuthorId: {AuthorId} - DocumentId: {DocumentId}", authorId, documentId);

        var document = await this._context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        if (document == null)
        {
            throw new BadHttpRequestException("Document not found");
        }

        if (authorId == null)
        {
            throw new UnauthorizedAccessException("Not authenticated");
        }

        if (document.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        return this._mapper.Map<DocumentDTO>(document);
    }

    public async Task<DocumentDTO?> GetDocumentPreview(Guid documentId)
    {
        this._logger.LogInformation("Get Document Preview - DocumentId: {DocumentId}", documentId);

        var document = await this._context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        if (document == null)
        {
            throw new BadHttpRequestException("Document not found");
        }

        if (document.IsPublished == true && document.IsArchived == false)
        {
            return this._mapper.Map<DocumentDTO>(document);
        }

        throw new BadHttpRequestException("Document not found");
    }

    public List<DocumentDTO> ListDocuments(Guid authorId, Guid? parentDocumentId)
    {
        this._logger.LogInformation("List Documents - AuthorId: {AuthorId} - ParentDocumentId: {ParentDocumentId}", authorId, parentDocumentId);

        var query = this._context.Documents
            .Where(d => d.AuthorId == authorId && d.ParentDocumentId == parentDocumentId && d.IsArchived == false)
            .OrderByDescending(d => d.CreatedAt);

        var documents = query.ToList();

        return this._mapper.Map<List<DocumentDTO>>(documents);
    }

    public async Task<DocumentDTO> UpdateDocument(Guid authorId, Guid documentId, UpdateDocumentDTO data)
    {
        this._logger.LogInformation("Update Document - AuthorId: {AuthorId} - DocumentId: {DocumentId} - Data: {Data}", authorId, documentId, data);

        var document = await this._context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        if (document is null)
        {
            throw new BadHttpRequestException("Document not found");
        }

        if (document.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        var updatedDocument = this._mapper.Map(data, document);

        this._context.Documents.Update(updatedDocument);
        await this._context.SaveChangesAsync();

        return this._mapper.Map<DocumentDTO>(updatedDocument);
    }

    public async Task<DocumentDTO> ArchiveDocument(Guid documentId, Guid authorId)
    {
        this._logger.LogInformation("Archive Document - AuthorId: {AuthorId} - DocumentId: {DocumentId}", authorId, documentId);

        var document =
            await this._context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        if (document is null)
        {
            throw new BadHttpRequestException("Document not found");
        }

        if (document.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        document.IsArchived = true;
        this._context.Documents.Update(document);
        await this.RecursiveArchiveDocuments(documentId);
        await this._context.SaveChangesAsync();

        return this._mapper.Map<DocumentDTO>(document);
    }

    public async Task<List<DocumentDTO>> ListArchiveDocuments(Guid authorId)
    {
        this._logger.LogInformation("List Archive Documents - AuthorId: {AuthorId}", authorId);

        var documents = await this._context.Documents
            .Where(d => d.AuthorId == authorId && d.IsArchived == true)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

        return this._mapper.Map<List<DocumentDTO>>(documents);
    }

    public async Task<DocumentDTO> RestoreDocument(Guid authorId, Guid documentId)
    {
        this._logger.LogInformation("Restore Document - AuthorId: {AuthorId} - DocumentId: {DocumentId}", authorId, documentId);

        var document = await this._context.Documents
            .Include(d => d.ParentDocument)
            .FirstOrDefaultAsync(d => d.Id == documentId);
        if (document == null)
        {
            throw new BadHttpRequestException("Document not found");
        }

        if (document.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        document.IsArchived = false;
        this._context.Documents.Update(document);
        if (document.ParentDocument?.IsArchived == true)
        {
            await this.RecursiveRestoreDocuments(document.Id);
        }

        await this.RecursiveRestoreDocuments(document.Id);

        await this._context.SaveChangesAsync();

        return this._mapper.Map<DocumentDTO>(document);
    }

    public async Task<DocumentDTO> DeleteDocument(Guid authorId, Guid documentId)
    {
        this._logger.LogInformation("Delete Document - AuthorId: {AuthorId} - DocumentId: {DocumentId}", authorId, documentId);

        var document = await this._context.Documents
            .FirstOrDefaultAsync(d => d.Id == documentId);
        if (document == null)
        {
            throw new BadHttpRequestException("Document not found");
        }

        if (document.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        this._context.Documents.Remove(document);
        await this._context.SaveChangesAsync();

        return this._mapper.Map<DocumentDTO>(document);
    }

    public async Task<DocumentDTO> RemoveDocumentIcon(Guid authorId, Guid documentId)
    {
        this._logger.LogInformation("Remove Document Icon - AuthorId: {AuthorId} - DocumentId: {DocumentId}", authorId, documentId);

        var document = await this._context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        if (document is null)
        {
            throw new BadHttpRequestException("Document not found");
        }

        if (document.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        document.Icon = null;
        this._context.Documents.Update(document);
        await this._context.SaveChangesAsync();

        return this._mapper.Map<DocumentDTO>(document);
    }

    public async Task<DocumentDTO> RemoveDocumentCoverImage(Guid authorId, Guid documentId)
    {
        this._logger.LogInformation("Remove Document Cover Image - AuthorId: {AuthorId} - DocumentId: {DocumentId}", authorId, documentId);

        var document = await this._context.Documents.FirstOrDefaultAsync(d => d.Id == documentId);
        if (document is null)
        {
            throw new BadHttpRequestException("Document not found");
        }

        if (document.AuthorId != authorId)
        {
            throw new UnauthorizedAccessException("Unauthorized");
        }

        document.CoverImage = null;
        this._context.Documents.Update(document);
        await this._context.SaveChangesAsync();

        return this._mapper.Map<DocumentDTO>(document);
    }

    private async Task RecursiveArchiveDocuments(Guid documentId)
    {
        var documents = await this._context.Documents
            .Where(d => d.ParentDocumentId == documentId)
            .ToListAsync();

        foreach (var doc in documents)
        {
            doc.IsArchived = true;
            this._context.Documents.Update(doc);
            await this.RecursiveArchiveDocuments(doc.Id);
        }
    }

    private async Task RecursiveRestoreDocuments(Guid documentId)
    {
        var documents = await this._context.Documents
            .Where(d => d.ParentDocumentId == documentId)
            .Include(d => d.ParentDocument)
            .ToListAsync();

        foreach (var doc in documents)
        {
            doc.IsArchived = false;
            this._context.Documents.Update(doc);
            await this.RecursiveRestoreDocuments(doc.Id);
        }
    }
}