using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.Grpc.Models
{
    [DataContract]
    public class DeleteApiKeyRequest
    {
        [DataMember(Order = 1)]
        public string ApiKeyId { get; set; }
    }
}
