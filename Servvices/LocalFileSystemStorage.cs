//using System.IO;
//using System.Threading.Tasks;
//using BlobStorageService.Models;



//namespace BlobStorageService.Services
//{
//    public class LocalFileSystemStorage : IStorageBackend
//    {
//        private readonly string _storagePath;

//        public LocalFileSystemStorage(string storagePath)
//        {
//            _storagePath = storagePath;
//            // التأكد من وجود المجلد
//            Directory.CreateDirectory(_storagePath);
//        }

//        public async Task SaveBlobAsync(Blob blob)
//        {
//            var filePath = Path.Combine(_storagePath, blob.Id);
//            await File.WriteAllTextAsync(filePath, blob.Data); // حفظ البيانات كملف نصي
//        }

//        public async Task<Blob> GetBlobAsync(string id)
//        {
//            var filePath = Path.Combine(_storagePath, id);
//            if (!File.Exists(filePath)) return null;  // إذا لم يكن الملف موجودًا

//            var data = await File.ReadAllTextAsync(filePath);
//            return new Blob
//            {
//                Id = id,
//                Data = data,
//                Size = data.Length,
//                CreatedAt = DateTime.UtcNow
//            };
//        }
//    }
//}


using System.IO;  // للوصول إلى System.IO.File
using System.Threading.Tasks;
using BlobStorageService.Models;  // للوصول إلى BlobStorageService.Models.File

namespace BlobStorageService.Services
{
    public class LocalFileSystemStorage : IStorageBackend
    {
        private readonly string _storagePath;

        public LocalFileSystemStorage(string storagePath)
        {
            _storagePath = storagePath;
            // التأكد من وجود المجلد
            Directory.CreateDirectory(_storagePath);
        }

        public async Task SaveBlobAsync(Blob blob)
        {
            var filePath = Path.Combine(_storagePath, blob.Id);
            // استخدام System.IO.File
            await System.IO.File.WriteAllTextAsync(filePath, blob.Data); // حفظ البيانات كملف نصي
        }

        public async Task<Blob> GetBlobAsync(string id)
        {
            var filePath = Path.Combine(_storagePath, id);
            if (!System.IO.File.Exists(filePath)) return null;  // إذا لم يكن الملف موجودًا

            var data = await System.IO.File.ReadAllTextAsync(filePath);
            return new Blob
            {
                Id = id,
                Data = data,
                Size = data.Length,
                CreatedAt = DateTime.UtcNow
            };
        }

        // تحميل ملف إلى النظام المحلي
        public async Task UploadFileAsync(string filePath, string fileName)
        {
            var destinationPath = Path.Combine(_storagePath, fileName);

            // التأكد من وجود الدليل، إذا لم يكن موجودًا سيتم إنشاؤه
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }

            // نسخ الملف إلى المسار المحلي
            await Task.Run(() => System.IO.File.Copy(filePath, destinationPath)); // استخدام System.IO.File
        }

        // تحميل ملف من النظام المحلي
        public async Task DownloadFileAsync(string fileName, string downloadPath)
        {
            var filePath = Path.Combine(_storagePath, fileName);

            // التأكد من أن الملف موجود
            if (System.IO.File.Exists(filePath))
            {
                await Task.Run(() => System.IO.File.Copy(filePath, downloadPath)); // استخدام System.IO.File
            }
        }
    }
}
