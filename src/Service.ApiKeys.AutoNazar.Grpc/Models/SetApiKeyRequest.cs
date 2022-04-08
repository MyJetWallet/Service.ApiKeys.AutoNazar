using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Service.ApiKeys.AutoNazar.Grpc.Models
{
    [DataContract]
    public class SetApiKeyRequest
    {
        [DataMember(Order = 1)]
        public string EncryptionKeyId { get; set; }

        [DataMember(Order = 2)]
        public string ApiKey { get; set; }

        [DataMember(Order = 3)]
        public string ApiKeyId { get; set; }

        [DataMember(Order = 4)]
        public string PrivateKey { get; set; }

        [DataMember(Order = 5)]
        public string CheckWord { get; set; }
        [DataMember(Order = 6)] 
        public string ApplicationUri { get; set; }
    }
}
