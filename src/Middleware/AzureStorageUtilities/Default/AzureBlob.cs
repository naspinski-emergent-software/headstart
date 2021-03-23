using AzureStorageUtilities.Interfaces;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageUtilities.Default
{
    public class AzureBlob : IBlob
    {
        public CloudBlobClient Client { get; }

        public CloudBlobContainer BlobContainer { get; }


        public AzureBlob(CloudStorageAccount account, string containerName)
        {
            Client = account.CreateCloudBlobClient();
            BlobContainer  = Client.GetContainerReference(containerName);
            BlobContainer.CreateIfNotExistsAsync();
        }

        public async Task<Stream> GetStreamWithFallbackAsync(string blobName, string defaultBlobName)
        {
            var blob = BlobContainer.GetBlockBlobReference(blobName);
            if (!(await blob.ExistsAsync()))
                blob = BlobContainer.GetBlockBlobReference(defaultBlobName);

            return await blob.OpenReadAsync();
        }
    }
}
