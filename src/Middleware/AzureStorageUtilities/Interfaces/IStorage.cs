using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;

namespace AzureStorageUtilities.Interfaces
{
    public interface IStorage
    {
        CloudStorageAccount Account { get; }
        Dictionary<string, ITable> Tables { get; }
        Dictionary<string, IBlob> BlobContainers { get; }
        ITable GetTable(string tableName);
        IBlob GetBlobContainer(string blobContainer);
    }
}