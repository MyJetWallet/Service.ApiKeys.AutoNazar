using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.Domain.Models.ApiKeys
{
    //This is record that is created on admin panel
    [DataContract]
    public class ApiKeyRecord
    {
        [DataMember(Order = 1)] public string ApiKeyId { get; set; }
        [DataMember(Order = 2)] public string EncryptionKeyId { get; set; }
        [DataMember(Order = 3)] public string ApplicationName { get; set; }
        [DataMember(Order = 4)] public string ApplicationUri { get; set; }
    }
}