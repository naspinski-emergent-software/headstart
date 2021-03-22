using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AzureStorageUtilities.Interfaces
{
    public interface ITable
    {
        CloudTableClient Client { get; }
        CloudTable Table { get; }

        Task<TableResult> AddAsync<T>(T entity) where T : ITableEntity, new();
        Task<IList<TableResult>> AddAsync<T>(IEnumerable<T> entities) where T : ITableEntity, new();

        Task<TableResult> SaveAsync<T>(T entity) where T : ITableEntity, new();
        Task<IList<TableResult>> SaveAsync<T>(IEnumerable<T> entities) where T : ITableEntity, new();

        Task<T> GetAsync<T>(string partitionKey, string rowKey) where T : ITableEntity, new();
        Task<IList<T>> GetAsync<T>(string partitionKey) where T : ITableEntity, new();

        Task DeleteAsync<T>(T entity) where T : ITableEntity, new();
        Task DeleteAsync<T>(string partitionKey, string rowKey) where T : ITableEntity, new();
    }
}
