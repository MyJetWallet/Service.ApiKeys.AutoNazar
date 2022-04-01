using MyNoSqlServer.Abstractions;
using Service.ApiKeys.AutoNazar.Domain.Models;

namespace Service.ApiKeys.AutoNazar.Jobs
{
    public class ApiKeyRecordNoSql : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-adminpanel-apikey-record";

        public ApiKeyRecord ApiKey { get; set; }
        public static string GeneratePartitionKey() => "ApiKey";
        public static string GenerateRowKey(string name) => $"{name}";

        public static ApiKeyRecordNoSql Create(ApiKeyRecord apiKey)
        {
            return new ApiKeyRecordNoSql
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(apiKey.ApplicationName),
                ApiKey = apiKey
            };
        }
    }
}
