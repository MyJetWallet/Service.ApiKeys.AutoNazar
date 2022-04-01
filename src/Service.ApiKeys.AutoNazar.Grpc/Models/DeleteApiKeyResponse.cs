using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.Grpc.Models
{
    [DataContract]
    public class DeleteApiKeyResponse
    {
        [DataMember(Order = 1)]
        public ErrorResponse Error { get; set; }
    }
}
