using static System.Net.Mime.MediaTypeNames;

namespace BC_Web2.Models.Domain
{
    public class Album
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Image> Images { get; set; }
    }
}
