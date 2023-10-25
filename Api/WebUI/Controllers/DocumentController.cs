namespace NotionBackend.Api.WebUI.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NotionBackend.Api.Application.Services.Document;
using NotionBackend.Api.Domain.DTOs.Auth;
using NotionBackend.Api.Domain.DTOs.Document;

[Route("api/documents")]
[ApiController]
public class DocumentController : ControllerBase
{
    private readonly IDocumentService _documentService;

    public DocumentController(IDocumentService documentService)
    {
        this._documentService = documentService;
    }

    [Authorize]
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<DocumentDTO>> CreateDocument([FromForm] CreateDocumentDTO body)
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var newDocument = await this._documentService.CreateDocument(user.UserId, body);
        return Ok(newDocument);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<DocumentDTO>> GetDocument([FromRoute] Guid id)
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var document = await this._documentService.GetDocument(user?.UserId, id);
        return Ok(document);
    }

    [HttpGet("{id}/preview")]
    public async Task<ActionResult<DocumentDTO>> GetDocumentPreview([FromRoute] Guid id)
    {
        var document = await this._documentService.GetDocumentPreview(id);
        return Ok(document);
    }

    [Authorize]
    [HttpGet("archive")]
    public async Task<ActionResult<List<DocumentDTO>>> ListArchiveDocuments()
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var documents = await this._documentService.ListArchiveDocuments(user.UserId);
        return Ok(documents);
    }

    [Authorize]
    [HttpGet]
    public ActionResult<List<DocumentDTO>> ListDocuments([FromQuery] ListDocumentQueryDTO query)
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var documents = this._documentService.ListDocuments(user.UserId, query.ParentDocumentId);
        return Ok(documents);
    }

    [Authorize]
    [HttpPatch("{id}")]
    // [Consumes("multipart/form-data")]
    public async Task<ActionResult<DocumentDTO>> UpdateDocument([FromRoute] Guid id, [FromBody] UpdateDocumentDTO form)
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var document = await this._documentService.UpdateDocument(user.UserId, id, form);
        return Ok(document);
    }

    [Authorize]
    [HttpPatch("{id}/remove/icon")]
    public async Task<ActionResult<DocumentDTO>> RemoveDocumentIcon([FromRoute] Guid id)
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var document = await this._documentService.RemoveDocumentIcon(user.UserId, id);
        return Ok(document);
    }

    [Authorize]
    [HttpPatch("{id}/remove/image")]
    public async Task<ActionResult<DocumentDTO>> RemoveDocumentCoverImage([FromRoute] Guid id)
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var document = await this._documentService.RemoveDocumentCoverImage(user.UserId, id);
        return Ok(document);
    }

    [Authorize]
    [HttpPost("archive")]
    public async Task<ActionResult<DocumentDTO>> ArchiveDocument([FromBody] ArchiveDocumentDTO body)
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var document = await this._documentService.ArchiveDocument(body.DocumentId, body.AuthorId ?? user.UserId);
        return Ok(document);
    }

    [Authorize]
    [HttpPost("restore")]
    public async Task<ActionResult<DocumentDTO>> RestoreDocument([FromBody] ArchiveDocumentDTO body)
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var document = await this._documentService.RestoreDocument(body.AuthorId ?? user.UserId, body.DocumentId);
        return Ok(document);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<DocumentDTO>> DeleteDocument([FromRoute] Guid id)
    {
        var user = (RequestUser)HttpContext.Items["User"];
        var document = await this._documentService.DeleteDocument(user.UserId, id);
        return Ok(document);
    }
}