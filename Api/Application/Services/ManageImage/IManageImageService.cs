namespace NotionBackend.Api.Application.Services.ManageImage;

public interface IManageImageService
{
    Task<string> UploadFile(IFormFile formFile);

    string GetFile(string filename);

    void DeleteImage(string filename);
}
