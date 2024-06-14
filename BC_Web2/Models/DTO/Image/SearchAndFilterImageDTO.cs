namespace BC_Web2.Models.DTO.Image
{
    public class SearchAndFilterImageDTO
    {
        public string? Query { get; set; }
        public string? FileExtension { get; set; }
        public long? MinSize { get; set; }
        public long? MaxSize { get; set; }
    }
}
