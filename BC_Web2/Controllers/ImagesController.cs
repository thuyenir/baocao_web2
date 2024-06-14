using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using BC_Web2.Models.Interfaces;
using BC_Web2.Models.DTO.Image;
using BC_Web2.Models.Domain;

namespace BC_Web2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImagesController : ControllerBase
    {
        private readonly IImageRepository _imageRepository;
        public ImagesController(IImageRepository imageRepository)
        {
            _imageRepository = imageRepository;
        }
        //Lấy tất cả danh sách ảnh
        [HttpGet]
        [Route("Get-all-image")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult GetAllImages()
        {
            var allImage = _imageRepository.GetAllImages();
            return Ok(allImage);
        }
        //Lấy ảnh theo id
        [HttpGet]
        [Route("Get-image-by-id/{id}")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult GetImageById(int id)
        {
            var image = _imageRepository.GetImageById(id);
            return Ok(image);
        }
        //Thêm ảnh
        [HttpPost]
        [Route("Upload-image")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult Upload([FromForm] ImageUploadRequestDTO reques)
        {
            ValidateFileUpload(reques);
            if (ModelState.IsValid)
            {
                var imageDTO = new Image
                {
                    File = reques.File,
                    FileExtention = Path.GetExtension(reques.File.FileName),
                    FileSizeInBytes = reques.File.Length,
                    FileName = reques.FileName,
                    FileDescription = reques.FileDescription,
                    AlbumId = reques.AlbumId,

                };
                _imageRepository.Upload(imageDTO);
                return Ok(imageDTO);
            }
            return BadRequest(ModelState);

        }
        private void ValidateFileUpload(ImageUploadRequestDTO reques)
        {
            var allowEx = new string[] { ".jpg", ".jpeg", ".png" };
            if (!allowEx.Contains(Path.GetExtension(reques.File.FileName)))
            {
                ModelState.AddModelError("file", "Error");
            }
            if (reques.File.Length > 1040000)
            {
                ModelState.AddModelError("file", "Error");
            }
        }
        //Cập nhật thông tin ảnh như tên, mô tả.....
        [HttpPut]
        [Route("Update-image/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateImage(int id, [FromBody] UpdateImageDTO updatedImage)
        {
            if (updatedImage == null || id != updatedImage.Id)
            {
                return BadRequest();
            }

            var existingImage = _imageRepository.GetImageById(id);
            if (existingImage == null)
            {
                return NotFound();
            }

            var updated = _imageRepository.UpdateImage(id, updatedImage);
            if (updated == null)
            {
                return StatusCode(500, "A problem happened.");
            }

            return Ok(updated);
        }
        //Tải ảnh
        [HttpGet]
        [Route("Download-image/{id}")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult DownloadImage(int id)
        {
            var result = _imageRepository.DownloadFile(id);
            return File(result.Item1, result.Item2, result.Item3);
        }
        //Xóa dữ liệu ảnh
        [HttpDelete]
        [Route("Delete-image/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteImage(int id)
        {
            var success = _imageRepository.DeleteImage(id);
            return Ok(success);
        }
        //Chia sẻ ảnh với người khác qua email
        [HttpPost]
        [Route("Share")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult ShareImage([FromBody] ShareImageDTO shareImageDto)
        {
            var shareLink = _imageRepository.GenerateShareLink(shareImageDto.ImageId);
            if (shareLink == null)
            {
                return NotFound();
            }

            var result = SendEmailAsync(shareImageDto.RecipientEmail, "Image Share", $"You can view the image using this link: {shareLink}");
            if (!result)
            {
                return StatusCode(500, "A problem happened while sending the email.");
            }

            return Ok(new { message = "Share link sent successfully" });
        }
        private bool SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("thuyenir@gmail.com", "sinhvien2003@"),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("thuyenir@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch
            {
                return false;
            }
        }
        //Tìm kiếm và lọc ảnh
        [HttpGet]
        [Route("Search-and-sortby-image")]
        [Authorize(Roles = "User,Admin")]
        public IActionResult SearchAndFilterImages([FromQuery] SearchAndFilterImageDTO filter)
        {
            var images = _imageRepository.SearchAndFilterImages(filter.Query, filter.FileExtension, filter.MinSize, filter.MaxSize);
            return Ok(images);
        }

    }
}
