using MyNoSqlServer.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.ApiKeys.AutoNazar.Tests
{
    public class InMemoryMyNoSqlServerDataWriter<T> :
        IMyNoSqlServerDataWriter<T> where T : IMyNoSqlDbEntity, new()
    {
        private readonly Dictionary<(string, string), T> _storage;

        public InMemoryMyNoSqlServerDataWriter()
        {
            _storage = new Dictionary<(string, string), T>();
        }
        public ValueTask<ITransactionsBuilder<T>> BeginTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask BulkInsertOrReplaceAsync(IEnumerable<T> entity, DataSynchronizationPeriod dataSynchronizationPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndBulkInsertAsync(IEnumerable<T> entity, DataSynchronizationPeriod dataSynchronizationPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndBulkInsertAsync(string partitionKey, IEnumerable<T> entity, DataSynchronizationPeriod dataSynchronizationPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndKeepLastRecordsAsync(string partitionKey, int amount)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndKeepMaxPartitions(int maxAmount)
        {
            throw new NotImplementedException();
        }

        public ValueTask CleanAndKeepMaxRecords(string partitionKey, int maxAmount)
        {
            throw new NotImplementedException();
        }

        public ValueTask<T> DeleteAsync(string partitionKey, string rowKey)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<T> GetAllAsync(int bulkRecordsCount)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<T>> GetAsync()
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<T>> GetAsync(string partitionKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask<T> GetAsync(string partitionKey, string rowKey)
        {
            if (_storage.TryGetValue((partitionKey, rowKey), out var val))
                return ValueTask.FromResult(val);

            return ValueTask.FromResult<T>(default);
        }

        public ValueTask<int> GetCountAsync(string partitionKey)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<T>> GetHighestRowAndBelow(string partitionKey, string rowKeyFrom, int amount)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IReadOnlyList<T>> GetMultipleRowKeysAsync(string partitionKey, IEnumerable<string> rowKeys)
        {
            throw new NotImplementedException();
        }

        public ValueTask InsertAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public ValueTask InsertOrReplaceAsync(T entity)
        {
            _storage[(entity.PartitionKey, entity.RowKey)] = entity;

            return ValueTask.CompletedTask;
        }

        public ValueTask<OperationResult> MergeAsync(string partitionKey, string rowKey, Func<T, bool> updateCallback, DataSynchronizationPeriod syncPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }

        public ValueTask<IEnumerable<T>> QueryAsync(string query)
        {
            throw new NotImplementedException();
        }

        public ValueTask<OperationResult> ReplaceAsync(string partitionKey, string rowKey, Func<T, bool> updateCallback, DataSynchronizationPeriod syncPeriod = DataSynchronizationPeriod.Sec5)
        {
            throw new NotImplementedException();
        }
    }
}
