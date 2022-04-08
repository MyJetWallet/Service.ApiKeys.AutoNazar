using MyNoSqlServer.Abstractions;
using Service.ApiKeys.AutoNazar.Domain.Models.ApiKeys;
using System;
using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.NoSql
{
    public class ApiKeyNoSqlEntity : MyNoSqlDbEntity
    {
        public const string TableName = "myjetwallet-api-security-manager-apikeys";

        public ApiKey ApiKey { get; set; }
        public static string GeneratePartitionKey() => "AutoNazar";
        public static string GenerateRowKey(string id) => id;

        public static ApiKeyNoSqlEntity Create(ApiKey apiKey)
        {
            return new ApiKeyNoSqlEntity
            {
                PartitionKey = GeneratePartitionKey(),
                RowKey = GenerateRowKey(apiKey.Id),
                ApiKey = apiKey
            };
        }
    }
}
