using BC_Web2.Models.DTO.Album;
using BC_Web2.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BC_Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AlbumsController : ControllerBase
    {
        private readonly IAlbumRepository _albumRepository;
        public AlbumsController(IAlbumRepository albumRepository)
        {
            _albumRepository = albumRepository;
        }
        // Lấy danh sách album
        [HttpGet("Get-all-albums")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAlbums()
        {
            var albums = _albumRepository.GetAllAlbums();
            return Ok(albums);
        }
        // Lấy album theo id
        [HttpGet]
        [Route("Get-album-by-id/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAlbumById(int id)
        {
            var album = _albumRepository.GetAlbumById(id);
            return Ok(album);
        }
        // Thêm album
        [HttpPost]
        [Route("Add-album")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddAlbum(AddAlbumDTO album)
        {
            var addAlbum = _albumRepository.AddAlbum(album);
            return Ok(addAlbum);
        }
        // Sửa album
        [HttpPut]
        [Route("Update-album/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateAlbum(int id, UpdateAlbumDTO album)
        {
            var upAlbum = _albumRepository.UpdateAlbum(id, album);
            return Ok(upAlbum);
        }
        //Xóa album
        [HttpDelete]
        [Route("Delete-album/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAlbum(int id)
        {
            var album = _albumRepository.DeleteAlbum(id);
            return Ok(album);
        }
    }
}

