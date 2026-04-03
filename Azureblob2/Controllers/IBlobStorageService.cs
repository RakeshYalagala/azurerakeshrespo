using Azureblob2.Data;

namespace Azureblob2.Controllers
{
    public interface IBlobStorageService
    {
        Task<IEnumerable<string>> ListBlobsAsync(CancellationToken cancellationToken = default);
        Task<Stream> DownloadAsync(string blobName, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string blobName, CancellationToken cancellationToken = default);
        Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default);
        Task<UserMaster> CreateUserAsync(UserMaster userMaster);
        Task<List<UserMaster>?> GetUsersAsync(string? id);
        Task<List<UserMaster>?> GetUserByEmailAsync(string? email);
    }
}