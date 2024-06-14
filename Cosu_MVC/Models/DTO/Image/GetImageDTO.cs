using System.ComponentModel.DataAnnotations.Schema;

namespace Cosu_MVC.Models.DTO.Image
{
    public class GetImageDTO
    {
        public int Id { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string? FileDescription { get; set; }
        public string FileExtention { get; set; }
        public long FileSizeInBytes { get; set; }
        public string FilePath { get; set; }
        public string AlbumName { get; set; }
    }
}
