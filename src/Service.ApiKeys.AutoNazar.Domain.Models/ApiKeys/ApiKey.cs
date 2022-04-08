using System;
using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.Domain.Models.ApiKeys
{
    [DataContract]
    public class ApiKey
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public string ApiKeyValue { get; set; }
        [DataMember(Order = 3)] public DateTime RegisterDate { get; set; }
        [DataMember(Order = 4)] public DateTime UpdateDate { get; set; }
        [DataMember(Order = 5)] public string EncryptionKeyId { get; set; }
        [DataMember(Order = 6)] public string PrivateKeyValue { get; set; }
        [DataMember(Order = 7)] public string CheckWord { get; set; }

        public static ApiKey Create(string id, string apiKey, string checkWord, string encryptionKeyId,
            string privateKeyValue, DateTime date)
        {
            return new ApiKey()
            {
                ApiKeyValue = apiKey,
                CheckWord = checkWord,
                EncryptionKeyId = encryptionKeyId,
                PrivateKeyValue = privateKeyValue,
                Id = id,
                UpdateDate = date,
                RegisterDate = date,
            };
        }
    }
}
