using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.Grpc.Models
{
    [DataContract]
    public class GetApiKeyIdsResponse
    {
        [DataMember(Order = 1)]
        public IReadOnlyCollection<string> Ids { get; set; }

        [DataMember(Order = 2)]
        public ErrorResponse Error { get; set; }
    }
}
