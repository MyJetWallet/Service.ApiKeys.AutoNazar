using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.Grpc.Models
{
    [DataContract]
    public class SetEncryptionKeyResponse
    {
        [DataMember(Order = 1)]
        public ErrorResponse Error { get; set; }

        [DataMember(Order = 2)]
        public IReadOnlyCollection<string> ActivatedIds { get; set; }
    }
}