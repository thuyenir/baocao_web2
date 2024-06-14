using System.ComponentModel.DataAnnotations.Schema;

namespace BC_Web2.Models.Domain
{
    public class Image
    {
        public int Id { get; set; }
        [NotMapped]
        public IFormFile File { get; set; }
        public string FileName { get; set; }
        public string? FileDescription { get; set; }
        public string FileExtention { get; set; }
        public long FileSizeInBytes { get; set; }
        public string FilePath { get; set; }
        //One to many
        public int AlbumId { get; set; }
        public Album Album { get; set; }
    }
}
