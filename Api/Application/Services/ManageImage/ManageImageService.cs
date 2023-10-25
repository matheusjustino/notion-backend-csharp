namespace NotionBackend.Api.Application.Services.ManageImage;

public class ManageImageService : IManageImageService
{
    private readonly ILogger<ManageImageService> _logger;

    public ManageImageService(ILogger<ManageImageService> logger)
    {
        this._logger = logger;
    }

    public async Task<string> UploadFile(IFormFile formFile)
    {
        try
        {
            var filename = $"{Path.GetFileNameWithoutExtension(formFile.FileName)}-{DateTime.Now.Ticks.ToString()}{Path.GetExtension(formFile.FileName)}";
            var filePath = GetFilePath(filename);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await formFile.CopyToAsync(fileStream);

            return filename;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex.Message, ex);
            throw new BadHttpRequestException("Error when trying to save file");
        }
    }

    public void DeleteImage(string filename)
    {
        try
        {
            var filePath = GetFilePath(filename);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex.Message, ex);
            throw new BadHttpRequestException("Error when trying to delete file");
        }
    }

    public string GetFile(string filename)
    {
        return GetFilePath(filename);
    }

    private static string GetFilePath(string filename)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var staticContentDirectory = Path.Combine(currentDirectory, "Api/Uploads/StaticContent");
        if (!Directory.Exists(staticContentDirectory))
        {
            Directory.CreateDirectory(staticContentDirectory);
        }

        return Path.Combine(staticContentDirectory, filename);
    }
}