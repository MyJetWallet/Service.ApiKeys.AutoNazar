using System.Runtime.Serialization;

namespace Service.ApiKeys.AutoNazar.Grpc.Models
{
    [DataContract]
    public class SetEncryptionKeyRequest
    {
        [DataMember(Order = 1)]
        public string Id { get; set; }

        [DataMember(Order = 2)]
        public string EncryptionKey { get; set; }

        [DataMember(Order = 3)]
        public string CheckWord { get; set; }

        [DataMember(Order = 4)]
        public string ServiceUri { get; set; }

        [DataMember(Order = 5)]
        public string ServiceName { get; set; }
    }
}