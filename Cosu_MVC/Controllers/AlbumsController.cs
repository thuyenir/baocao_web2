using Microsoft.AspNetCore.Mvc;
using Cosu_MVC.Models.DTO.Album;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Cosu_MVC.Models;

namespace Cosu_MVC.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthRepository _api;
        public AlbumsController(IHttpClientFactory httpClientFactory, IAuthRepository api)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7029/api/");
            _api = api;
        }
        private void SetAuthorizationHeader()
        {
            var token = HttpContext.Session.GetString("JWToken");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
        // Danh sách album
        public async Task<IActionResult> ListAlbums()
        {
            List<GetAlbumDTO> response = new List<GetAlbumDTO>();
            try
            {
                SetAuthorizationHeader();
                var httpResponseMess = await _httpClient.GetAsync("Albums/Get-all-albums");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<GetAlbumDTO>>());
            }
            catch (Exception ex)
            {
            }
            return View(response);
        }
        // Hiển thị album theo id (chi tiết album)
        public async Task<IActionResult> AlbumId(int id)
        {
            GetAlbumDTO response = new GetAlbumDTO();
            try
            {
                SetAuthorizationHeader();
                var httpResponseMess = await _httpClient.GetAsync("Albums/Get-album-by-id/"+id);
                httpResponseMess.EnsureSuccessStatusCode();
                var stringResponbody = await httpResponseMess.Content.ReadAsStringAsync();
                response = await httpResponseMess.Content.ReadFromJsonAsync<GetAlbumDTO>();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching the album details.";
            }
            return View(response);
        }
        // Thêm album
        public IActionResult AddAlbum()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddAlbum(AddAlbumDTO album)
        {
            if (!ModelState.IsValid)
            {
                return View(album);
            }
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PostAsJsonAsync("Albums/Add-album", album);
                response.EnsureSuccessStatusCode();
                return RedirectToAction("ListAlbums");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while creating the album.";
                return View(album);
            }
        }
        // Sửa album
        public async Task<IActionResult> EditAlbum(int id)
        {
            UpdateAlbumDTO album = new UpdateAlbumDTO();
            try
            {
                SetAuthorizationHeader();
                var httpResponse = await _httpClient.GetAsync($"Albums/Get-album-by-id/{id}");
                httpResponse.EnsureSuccessStatusCode();
                var albumData = await httpResponse.Content.ReadFromJsonAsync<GetAlbumDTO>();

                if (albumData != null)
                {
                    album.Id = albumData.Id;
                    album.Name = albumData.Name;
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while fetching the album for editing.";
            }
            return View(album);
        }
        [HttpPost]
        public async Task<IActionResult> EditAlbum(UpdateAlbumDTO album)
        {
            if (!ModelState.IsValid)
            {
                return View(album);
            }
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"Albums/Update-album/{album.Id}", album);
                response.EnsureSuccessStatusCode();
                return RedirectToAction("ListAlbums");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while updating the album.";
                return View(album);
            }
        }
        // Xóa album
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var httpResponseMess = await _httpClient.DeleteAsync("Albums/Delete-album/"+id);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("ListAlbums");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "An error occurred while deleting the album.";
            }
            return View("ListAlbums");
        }
    }
}

