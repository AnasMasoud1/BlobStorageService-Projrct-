using System.IO; // للوصول إلى System.IO.File
using System.Net.Http;
using System.Threading.Tasks;
using BlobStorageService.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Text;

namespace BlobStorageService.Services
{
    public class S3StorageBackend : IStorageBackend
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;

        public S3StorageBackend(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _endpoint = configuration["Minio:Url"];
            _accessKey = configuration["Minio:AccessKey"];
            _secretKey = configuration["Minio:SecretKey"];
            _bucketName = configuration["Minio:BucketName"];
        }

        // رفع ملف إلى خدمة S3
        public async Task SaveBlobAsync(Blob blob)
        {
            var filePath = Path.Combine(_bucketName, blob.Id); // تحديد مسار الملف داخل الدلو

            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(blob.Data));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var requestUri = $"{_endpoint}/{_bucketName}/{blob.Id}";

            var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = fileContent
            };

            // إضافة رؤوس التوثيق
            request.Headers.Add("x-amz-access-key", _accessKey);
            request.Headers.Add("x-amz-secret-key", _secretKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("File uploaded successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to upload file: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while uploading the file: {ex.Message}");
            }
        }

        // تنزيل ملف من خدمة S3
        public async Task<Blob> GetBlobAsync(string id)
        {
            var requestUri = $"{_endpoint}/{_bucketName}/{id}";
            try
            {
                var response = await _httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    return new Blob
                    {
                        Id = id,
                        Data = data,
                        Size = data.Length,
                        CreatedAt = DateTime.UtcNow
                    };
                }
                else
                {
                    Console.WriteLine($"Failed to retrieve file: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving the file: {ex.Message}");
            }

            return null;
        }

        // تنفيذ UploadFileAsync من واجهة IStorageBackend
        public async Task UploadFileAsync(string filePath, string fileName)
        {
            var fileContent = new StreamContent(System.IO.File.OpenRead(filePath)); // استخدم System.IO.File هنا
            fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var requestUri = $"{_endpoint}/{_bucketName}/{fileName}";
            var request = new HttpRequestMessage(HttpMethod.Put, requestUri)
            {
                Content = fileContent
            };

            // إضافة رؤوس التوثيق
            request.Headers.Add("x-amz-access-key", _accessKey);
            request.Headers.Add("x-amz-secret-key", _secretKey);

            try
            {
                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("File uploaded successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to upload file: {response.StatusCode}, {await response.Content.ReadAsStringAsync()}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while uploading the file: {ex.Message}");
            }
        }

        // تنفيذ DownloadFileAsync من واجهة IStorageBackend
        public async Task DownloadFileAsync(string fileName, string downloadPath)
        {
            var requestUri = $"{_endpoint}/{_bucketName}/{fileName}";
            try
            {
                var response = await _httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsByteArrayAsync();
                    await System.IO.File.WriteAllBytesAsync(downloadPath, data); // استخدم System.IO.File هنا
                    Console.WriteLine("File downloaded successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to download file: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while downloading the file: {ex.Message}");
            }
        }
    }
}
