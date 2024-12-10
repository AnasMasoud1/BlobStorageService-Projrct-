using Microsoft.AspNetCore.Mvc;
using BlobStorageService.Models;
using BlobStorageService.Services;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using System.Threading.Tasks;
using Serilog;

namespace BlobStorageService.Controllers
{
    [ApiController]
    [Route("v1/blobs")]
    public class BlobsController : ControllerBase
    {
        private readonly IStorageBackend _storageBackend;

        public BlobsController(IStorageBackend storageBackend)
        {
            _storageBackend = storageBackend;
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> StoreBlob([FromBody] Blob blob)
        {
            blob.CreatedAt = DateTime.UtcNow;
            blob.Size = Convert.FromBase64String(blob.Data).Length;
            await _storageBackend.SaveBlobAsync(blob);

            return Ok();
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> RetrieveBlob(string id)
        {
            var blob = await _storageBackend.GetBlobAsync(id);
            if (blob == null) return NotFound();

            return Ok(blob);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File not selected or empty.");
            }

            var tempFolder = "C:\\BlobStorage";
            Directory.CreateDirectory(tempFolder); // التأكد من أن المجلد المؤقت موجود
            var filePath = Path.Combine(tempFolder, file.FileName);

            try
            {
                // حفظ الملف في المسار المؤقت
                Log.Information("Saving file temporarily: {FilePath}", filePath);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // رفع الملف إلى Minio
                Log.Information("Uploading file to Minio: {FileName}", file.FileName);
                await _storageBackend.UploadFileAsync(filePath, file.FileName);

                // حذف الملف المؤقت
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    Log.Information("Temporary file deleted: {FilePath}", filePath);
                }

                return Ok("File uploaded successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while uploading the file.");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }



        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var downloadPath = Path.Combine("C:\\BlobStorage", fileName);
            await _storageBackend.DownloadFileAsync(fileName, downloadPath);

            return Ok(new { FilePath = downloadPath });
        }
    }
}
