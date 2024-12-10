using System.Threading.Tasks;
using BlobStorageService.Models;

namespace BlobStorageService.Services
{
    public interface IStorageBackend
    {
        Task SaveBlobAsync(Blob blob);  // لحفظ Blob
        Task<Blob> GetBlobAsync(string id); // لاسترجاع Blob باستخدام الـ id

        Task UploadFileAsync(string filePath, string fileName);
        Task DownloadFileAsync(string fileName, string downloadPath);
    }
}


