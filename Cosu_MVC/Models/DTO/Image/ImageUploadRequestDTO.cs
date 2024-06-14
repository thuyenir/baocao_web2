using System.ComponentModel.DataAnnotations;

namespace Cosu_MVC.Models.DTO.Image
{
    public class ImageUploadRequestDTO
    {
        [Required]
        public IFormFile File { get; set; }
        [Required]
        public string FileName { get; set; }
        public string? FileDescription { get; set; }
        public int AlbumId { get; set; }
    }
}
