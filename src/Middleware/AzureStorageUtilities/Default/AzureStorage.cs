using AzureStorageUtilities.Interfaces;
using Microsoft.WindowsAzure.Storage;
using System.Collections.Generic;

namespace AzureStorageUtilities.Default
{
    public class AzureStorage : IStorage
    {
        public CloudStorageAccount Account { get; }
        public Dictionary<string, ITable> Tables { get; } = new Dictionary<string, ITable>();
        public Dictionary<string, IBlob> BlobContainers { get; } = new Dictionary<string, IBlob>();

        public AzureStorage(string connectionString)
        {
            Account = CloudStorageAccount.Parse(connectionString);
        }

        public ITable GetTable(string tableName)
        {
            if (!Tables.ContainsKey(tableName))
                Tables.Add(tableName, new AzureTable(Account, tableName));
            return Tables[tableName];
        }

        public IBlob GetBlobContainer(string blobContainer)
        {
            if (!BlobContainers.ContainsKey(blobContainer))
                BlobContainers.Add(blobContainer, new AzureBlob(Account, blobContainer));
            return BlobContainers[blobContainer];
        }
    }
}
