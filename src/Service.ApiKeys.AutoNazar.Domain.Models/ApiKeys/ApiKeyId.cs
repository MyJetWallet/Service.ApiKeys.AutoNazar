using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.Domain.Models.ApiKeys
{
    [DataContract]
    public class ApiKeyId
    {
        [DataMember(Order = 1)] public string Id { get; set; }
        [DataMember(Order = 2)] public string EncryptionKeyId { get; set; }
    }
}
