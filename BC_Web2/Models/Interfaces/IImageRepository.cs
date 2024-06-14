using BC_Web2.Models.Domain;
using BC_Web2.Models.DTO.Image;

namespace BC_Web2.Models.Interfaces
{
    public interface IImageRepository
    {
        List<GetImageDTO> GetAllImages();
        GetImageDTO GetImageById(int id);
        Image UpdateImage(int id, UpdateImageDTO updatedImage);
        Image Upload(Image image);
        (byte[], string, string) DownloadFile(int Id);
        Image DeleteImage(int id);

        string GenerateShareLink(int imageId);
        List<Image> SearchAndFilterImages(string query, string fileExtension, long? minSize, long? maxSize);
    }
}
