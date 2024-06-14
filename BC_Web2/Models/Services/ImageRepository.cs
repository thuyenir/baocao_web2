using BC_Web2.Data;
using BC_Web2.Models.Domain;
using BC_Web2.Models.DTO.Image;
using BC_Web2.Models.Interfaces;

namespace BC_Web2.Models.Services
{
    public class ImageRepository : IImageRepository
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppDbContext _appDbContext;
        public ImageRepository(IWebHostEnvironment environment, IHttpContextAccessor contextAccessor, AppDbContext appDbContext)
        {
            _environment = environment;
            _contextAccessor = contextAccessor;
            _appDbContext = appDbContext;
        }
        public List<GetImageDTO> GetAllImages()
        {
            var allImage = _appDbContext.Images
                .Select(image => new GetImageDTO
                {
                    Id = image.Id,
                    FileName = image.FileName,
                    FileDescription = image.FileDescription,
                    FileExtention = image.FileExtention,
                    FileSizeInBytes = image.FileSizeInBytes,
                    FilePath = image.FilePath,
                    AlbumName = image.Album.Name
                }).ToList();
            return allImage;
        }
        public GetImageDTO GetImageById(int id)
        {
            var image = _appDbContext.Images
                .Where(img => img.Id == id)
                .Select(image => new GetImageDTO
                {
                    Id = image.Id,
                    FileName = image.FileName,
                    FileDescription = image.FileDescription,
                    FileExtention = image.FileExtention,
                    FileSizeInBytes = image.FileSizeInBytes,
                    FilePath = image.FilePath,
                    AlbumName = image.Album.Name
                }).FirstOrDefault();
            return image;
        }
        public Image Upload(Image image)
        {
            var localFilePath = Path.Combine(_environment.ContentRootPath, "Images", $"{image.FileName}{image.FileExtention}");
            using var stream = new FileStream(localFilePath, FileMode.Create);
            image.File.CopyTo(stream);
            var urlFilePath = $"{_contextAccessor.HttpContext.Request.Scheme}://{_contextAccessor.HttpContext.Request.Host}{_contextAccessor.HttpContext.Request.PathBase}/Images/{image.FileName}{image.FileExtention}";
            image.FilePath = urlFilePath;
            _appDbContext.Images.Add(image);
            _appDbContext.SaveChanges();
            return image;
        }
        public Image UpdateImage(int id, UpdateImageDTO updatedImage)
        {
            var existingImage = _appDbContext.Images.Find(id);
            if (existingImage == null)
            {
                return null;
            }
            existingImage.FileName = updatedImage.FileName;
            existingImage.FileDescription = updatedImage.FileDescription;
            existingImage.AlbumId = updatedImage.AlbumId;
            _appDbContext.Images.Update(existingImage);
            _appDbContext.SaveChanges();

            return existingImage;
        }
        public (byte[], string, string) DownloadFile(int Id)
        {
            try
            {
                var FileById = _appDbContext.Images.Where(x => x.Id == Id).FirstOrDefault();

                var path = Path.Combine(_environment.ContentRootPath, "Images", $"{FileById.FileName}{FileById.FileExtention}");

                var stream = File.ReadAllBytes(path);
                var filename = FileById.FileName + FileById.FileExtention;

                return (stream, "application/octet-stream", filename);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public Image DeleteImage(int id)
        {
            var pu = _appDbContext.Images.FirstOrDefault(n => n.Id == id);
            if (pu != null)
            {
                _appDbContext.Images.Remove(pu);
                _appDbContext.SaveChanges();
            }
            return pu;
        }
        public string GenerateShareLink(int imageId)
        {
            var image = _appDbContext.Images.Find(imageId);
            if (image == null)
            {
                return null;
            }
            var shareLink = $"https://localhost:7277/{imageId}";
            return shareLink;
        }
        public List<Image> SearchAndFilterImages(string query, string fileExtension, long? minSize, long? maxSize)
        {
            var images = _appDbContext.Images.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                images = images.Where(i => i.FileName.Contains(query) || i.FileDescription.Contains(query));
            }

            if (!string.IsNullOrEmpty(fileExtension))
            {
                images = images.Where(i => i.FileExtention == fileExtension);
            }

            if (minSize.HasValue)
            {
                images = images.Where(i => i.FileSizeInBytes >= minSize.Value);
            }

            if (maxSize.HasValue)
            {
                images = images.Where(i => i.FileSizeInBytes <= maxSize.Value);
            }

            return images.ToList();
        }

    }
}
