using Service.ApiKeys.AutoNazar.Domain.Models.ApiKeys;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.Grpc.Models
{
    [DataContract]
    public class GetApiKeysResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<ApiKeyId> Ids { get; set; }

        [DataMember(Order = 2)]
        public ErrorResponse Error { get; set; }
    }
}
