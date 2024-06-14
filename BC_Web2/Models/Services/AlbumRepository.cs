using BC_Web2.Data;
using BC_Web2.Models.Domain;
using BC_Web2.Models.DTO.Album;
using BC_Web2.Models.Interfaces;

namespace BC_Web2.Models.Services
{
    public class AlbumRepository : IAlbumRepository
    {
        private readonly AppDbContext _db;
        public AlbumRepository(AppDbContext db)
        {
            _db = db;
        }
        public List<GetAlbumDTO> GetAllAlbums()
        {
            var allAl = _db.Albums.Select(pu => new GetAlbumDTO()
            {
                Id = pu.Id,
                Name = pu.Name,
                ImageName = pu.Images.Select(image => image.FileName).ToList()
            }).ToList();
            return allAl;
        }
        public GetAlbumDTO GetAlbumById(int id)
        {
            var alDTO = _db.Albums.Where(n => n.Id == id);
            var allAl = alDTO.Select(pu => new GetAlbumDTO()
            {
                Id = pu.Id,
                Name = pu.Name,
                ImageName = pu.Images.Select(image => image.FileName).ToList()
            }).FirstOrDefault();
            return allAl;
        }
        public AddAlbumDTO AddAlbum(AddAlbumDTO album)
        {
            var addAl = new Album
            {
                Name = album.Name,
            };
            _db.Albums.Add(addAl);
            _db.SaveChanges();
            return album;
        }
        public UpdateAlbumDTO UpdateAlbum(int id, UpdateAlbumDTO album)
        {
            var puDTO = _db.Albums.FirstOrDefault(n => n.Id == id);
            if (puDTO != null)
            {
                puDTO.Name = album.Name;

                _db.SaveChanges();
            }
            return album;
        }
        public Album DeleteAlbum(int id)
        {
            var pu = _db.Albums.FirstOrDefault(n => n.Id == id);
            if (pu != null)
            {
                _db.Albums.Remove(pu);
                _db.SaveChanges();
            }
            return pu;
        }
    }
}
