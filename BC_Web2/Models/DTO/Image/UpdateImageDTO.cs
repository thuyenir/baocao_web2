namespace BC_Web2.Models.DTO.Image
{
    public class UpdateImageDTO
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string? FileDescription { get; set; }
        public int AlbumId { get; set; }
    }
}
