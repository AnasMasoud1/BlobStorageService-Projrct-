//using Microsoft.Extensions.Configuration;
//using System;
//using System.IO;
//using System.Net.Http;
//using System.Threading.Tasks;

//namespace BlobStorageService.Services
//{
//    public class MinioStorageService 
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _endpoint;
//        private readonly string _accessKey;
//        private readonly string _secretKey;
//        private readonly string _bucketName;

//        public MinioStorageService(IConfiguration configuration)
//        {
//            _httpClient = new HttpClient();
//            _endpoint = configuration["S3:Endpoint"];
//            _accessKey = configuration["S3:AccessKey"];
//            _secretKey = configuration["S3:SecretKey"];
//            _bucketName = configuration["S3:BucketName"];
//        }

//        // رفع ملف
//        public async Task UploadFileAsync(string filePath)
//        {
//            var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
//            var requestUrl = $"{_endpoint}/{_bucketName}/{Path.GetFileName(filePath)}";

//            _httpClient.DefaultRequestHeaders.Clear();
//            _httpClient.DefaultRequestHeaders.Add("Authorization", $"AWS {_accessKey}:{_secretKey}");

//            var response = await _httpClient.PutAsync(requestUrl, fileContent);
//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception("Error uploading file.");
//            }
//        }

//        // تنزيل ملف
//        public async Task DownloadFileAsync(string fileName, string downloadPath)
//        {
//            var requestUrl = $"{_endpoint}/{_bucketName}/{fileName}";
//            var response = await _httpClient.GetAsync(requestUrl);
//            if (!response.IsSuccessStatusCode)
//            {
//                throw new Exception("Error downloading file.");
//            }

//            var fileBytes = await response.Content.ReadAsByteArrayAsync();
//            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None))
//            {
//                await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
//            }

//        }
//    }
//}


using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BlobStorageService.Models;
using Serilog; 

namespace BlobStorageService.Services
{
    public class MinioStorageService : IStorageBackend // تنفيذ الواجهة
    {
        private readonly HttpClient _httpClient;
        private readonly string _endpoint;
        private readonly string _accessKey;
        private readonly string _secretKey;
        private readonly string _bucketName;

        public MinioStorageService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _endpoint = configuration["S3:Endpoint"];
            _accessKey = configuration["S3:AccessKey"];
            _secretKey = configuration["S3:SecretKey"];
            _bucketName = configuration["S3:BucketName"];
        }

        public async Task UploadFileAsync(string filePath, string fileName)
        {
            try
            {
                // قراءة محتوى الملف
                var fileContent = new ByteArrayContent(System.IO.File.ReadAllBytes(filePath));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                // بناء URL الطلب
                var requestUrl = $"{_endpoint}/{_bucketName}/{fileName}";
                Log.Information("Uploading file to Minio. Request URL: {RequestUrl}", requestUrl);

                // تهيئة الطلب
                using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, requestUrl))
                {
                    requestMessage.Content = fileContent;

                    // إضافة الهيدر الخاص بـ Minio (تأكد أن هذه الطريقة تتوافق مع إعدادات Minio)
                    var authorizationHeader = $"AWS {_accessKey}:{_secretKey}";
                    requestMessage.Headers.Add("Authorization", authorizationHeader);

                    // تنفيذ الطلب
                    var response = await _httpClient.SendAsync(requestMessage);
                    Log.Information("Minio response status: {StatusCode}", response.StatusCode);

                    // تحقق من الاستجابة
                    if (!response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Log.Error("Error uploading file to Minio: {ResponseContent}", responseContent);
                        throw new Exception($"Error uploading file. StatusCode: {response.StatusCode}, Content: {responseContent}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "An error occurred while uploading the file to Minio.");
                throw;
            }
        }


        // تنزيل ملف
        public async Task DownloadFileAsync(string fileName, string downloadPath)
        {
            var requestUrl = $"{_endpoint}/{_bucketName}/{fileName}";
            var response = await _httpClient.GetAsync(requestUrl);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Error downloading file.");
            }

            var fileBytes = await response.Content.ReadAsByteArrayAsync();
            using (var fileStream = new FileStream(downloadPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
            }
        }

        // تنفيذ الدالة SaveBlobAsync من IStorageBackend
        public async Task SaveBlobAsync(Blob blob)
        {
            // في هذا المكان يمكنك تنفيذ منطق حفظ الـ Blob في Minio إذا رغبت
            // لكن يمكنك ترك هذه الدالة فارغة إذا كان الـ Minio لا يتعامل مع قاعدة البيانات
            throw new NotImplementedException();
        }

        // تنفيذ الدالة GetBlobAsync من IStorageBackend
        public async Task<Blob> GetBlobAsync(string id)
        {
            // هذه الدالة يمكن أن تسترجع الـ Blob من قاعدة البيانات إذا كان هذا هو المطلوب
            throw new NotImplementedException();
        }
    }
}
