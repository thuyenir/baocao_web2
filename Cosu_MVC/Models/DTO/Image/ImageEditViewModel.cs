using Cosu_MVC.Models.DTO.Album;

namespace Cosu_MVC.Models.DTO.Image
{
    public class ImageEditViewModel
    {
        public EditImageDTO EditedImage { get; set; }
        public IEnumerable<AlbumDTO> Albums { get; set; }
    }
}
