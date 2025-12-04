namespace CleanArchitectureTemplate.Application.Common.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName);
    Task<List<string>> UploadMultipleImagesAsync(List<(Stream fileStream, string fileName)> files);
    Task<bool> DeleteImageAsync(string publicId);
}
