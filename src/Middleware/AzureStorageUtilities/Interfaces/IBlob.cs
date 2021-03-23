using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageUtilities.Interfaces
{
    public interface IBlob
    {
        CloudBlobClient Client { get; }
        CloudBlobContainer BlobContainer { get; }

        Task<Stream> GetStreamWithFallbackAsync(string blobName, string defaultBlobName);
    }
}
