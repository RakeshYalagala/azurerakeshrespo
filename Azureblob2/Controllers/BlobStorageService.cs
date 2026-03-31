using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azureblob2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Azureblob2.Controllers
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient _containerClient;
        private readonly DBContext _context;


        public BlobStorageService(IOptions<BlobController.BlobStorageSettings> options,DBContext context)
        {
            _context = context;
            var settings = options.Value;
            var blobServiceClient = new BlobServiceClient(settings.ConnectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(settings.ContainerName);

            //_containerClient.CreateIfNotExists(PublicAccessType.None);
        }

        // Upload a file and return its URL
        public async Task<string> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
        {
            var blobName = $"{Guid.NewGuid()}_{file.FileName}";
            var blobClient = _containerClient.GetBlobClient(blobName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            await using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = blobHttpHeaders }, cancellationToken);

            return blobClient.Uri.ToString();
        }

        // Download a blob by name
        public async Task<Stream> DownloadAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync(cancellationToken))
                throw new FileNotFoundException($"Blob '{blobName}' not found.");

            var download = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return download.Value.Content;
        }

        // Delete a blob by name
        public async Task<bool> DeleteAsync(string blobName, CancellationToken cancellationToken = default)
        {
            var blobClient = _containerClient.GetBlobClient(blobName);
            return await blobClient.DeleteIfExistsAsync(cancellationToken: cancellationToken);
        }

        // List all blob names in the container
        public async Task<IEnumerable<string>> ListBlobsAsync(CancellationToken cancellationToken = default)
        {
            var blobs = new List<string>();
            await foreach (var blob in _containerClient.GetBlobsAsync(cancellationToken: cancellationToken))
            {
                blobs.Add(blob.Name);
            }
            return blobs;
        }
        public async Task<UserMaster> CreateUserAsync(UserMaster userMaster)
        {
            // Check if username already exists
            var existingUser = await _context.userMaster
                .FirstOrDefaultAsync(u => u.UserId == userMaster.UserId);

            if (existingUser != null)
                throw new InvalidOperationException("Username already exists.");

            var user = new UserMaster
            {
                UserId = userMaster.UserId,
                UserName = userMaster.UserName,
                UserEmail = userMaster.UserEmail,
                UserContactNo = userMaster.UserContactNo,
                Address = userMaster.Address,
                Password = BCrypt.Net.BCrypt.HashPassword(userMaster.Password),
                IsActive = true,
                RepMgrToken = userMaster.RepMgrToken,
                CreatedBy = userMaster.CreatedBy,
                CreatedOn = DateTime.Now,
                FailedAttempts = 0,
                IsBlocked = false,
                LastFailedAttempt = null,
                LockoutEndTime = null
            };

            _context.userMaster.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<List<UserMaster>?> GetUsersAsync(string? id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var user = await _context.userMaster
                                         .Where(x => x.UserId == id)
                                         .ToListAsync(); // always return a list
                return user;
            }

            return await _context.userMaster.ToListAsync();
        }
    }
}
