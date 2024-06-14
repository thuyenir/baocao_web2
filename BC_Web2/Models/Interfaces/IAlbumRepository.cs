using BC_Web2.Models.Domain;
using BC_Web2.Models.DTO.Album;

namespace BC_Web2.Models.Interfaces
{
    public interface IAlbumRepository
    {
        List<GetAlbumDTO> GetAllAlbums();
        GetAlbumDTO GetAlbumById(int id);
        AddAlbumDTO AddAlbum(AddAlbumDTO album);
        UpdateAlbumDTO UpdateAlbum(int id, UpdateAlbumDTO album);
        Album DeleteAlbum(int id);
    }
}
