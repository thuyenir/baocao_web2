using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Cosu_MVC.Models.DTO.Image;
using Cosu_MVC.Models.DTO.Album;
using Cosu_MVC.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Mime;
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using System.Text;

namespace Cosu_MVC.Controllers
{
    public class ImagesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthRepository _api;
        public ImagesController(IHttpClientFactory httpClientFactory, IAuthRepository api)
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
        //Danh sách ảnh
        public async Task<IActionResult> ListImages()
        {
            List<GetImageDTO> response = new List<GetImageDTO>();
            try
            {
                SetAuthorizationHeader();
                var httpResponseMess = await _httpClient.GetAsync("Images/Get-all-image");
                httpResponseMess.EnsureSuccessStatusCode();
                response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<GetImageDTO>>());
            }
            catch (Exception ex)
            {

            }
            return View(response);
        }
        //Hiển thị ảnh theo id(chi tiết ảnh)
        public async Task<IActionResult> ImageId(int id)
        {
            GetImageDTO response = new GetImageDTO();
            try
            {
                SetAuthorizationHeader();
                var httpResponseMess = await _httpClient.GetAsync("Images/Get-image-by-id/" + id);
                httpResponseMess.EnsureSuccessStatusCode();
                var stringResponbody = await httpResponseMess.Content.ReadAsStringAsync();
                response = await httpResponseMess.Content.ReadFromJsonAsync<GetImageDTO>();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
            }
            return View(response);
        }
        //Thêm ảnh
        public IActionResult UploadImage()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UploadImage(ImageUploadRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                SetAuthorizationHeader();
                using (var form = new MultipartFormDataContent())
                {
                    form.Add(new StreamContent(request.File.OpenReadStream())
                    {
                        Headers =
                            {
                                ContentLength = request.File.Length,
                                ContentType = new MediaTypeHeaderValue(request.File.ContentType)
                            }
                    }, "File", Path.GetFileName(request.File.FileName));

                    form.Add(new StringContent(request.FileName), "FileName");
                    form.Add(new StringContent(request.FileDescription ?? ""), "FileDescription");
                    form.Add(new StringContent(request.AlbumId.ToString()), "AlbumId");

                    var response = await _httpClient.PostAsync("Images/Upload-image", form);

                    if (!response.IsSuccessStatusCode)
                    {
                        return StatusCode((int)response.StatusCode);
                    }
                }
                return RedirectToAction("ListImages");
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
        // Sửa ảnh
        [HttpGet]
        public async Task<IActionResult> EditImage(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var httpResponseMess = await _httpClient.GetAsync($"Images/Get-image-by-id/{id}");
                httpResponseMess.EnsureSuccessStatusCode();
                var image = await httpResponseMess.Content.ReadFromJsonAsync<ImageDTO>();
                if (image == null)
                {
                    return NotFound();
                }
                var albumsResponse = await _httpClient.GetAsync("Albums/Get-all-albums");
                albumsResponse.EnsureSuccessStatusCode();
                var albums = await albumsResponse.Content.ReadFromJsonAsync<IEnumerable<AlbumDTO>>();

                var viewModel = new ImageEditViewModel
                {
                    EditedImage = new EditImageDTO
                    {
                        Id = image.Id,
                        Name = image.FileName,
                        Description = image.FileDescription,
                        AlbumId = image.AlbumId,
                    },
                    Albums = albums
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditImage(int id,[FromBody] EditImageDTO editedImage)
        {
            if (id != editedImage.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(editedImage);
            }

            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.PutAsJsonAsync($"Images/Update-image/{id}", editedImage);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListImages");
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View("Error");
            }
        }
        public async Task<IActionResult> DownloadImage(int id)
        {
            try
            {
                SetAuthorizationHeader();
                var response = await _httpClient.GetAsync($"Images/Download-image/{id}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType.MediaType;
                var fileName = response.Content.Headers.ContentDisposition.FileName;

                return File(content, contentType, fileName);
            }
            catch (Exception ex)
            {
                return View("Error");
            }
        }
    }
}





