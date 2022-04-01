using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Service.ApiKeys.AutoNazar.Grpc.Models
{
    [DataContract]
    public class ErrorResponse
    {
        [DataMember(Order = 1)]
        public string Message { get; set; }

        [DataMember(Order = 2)]
        public ErrorCode Code { get; set; }
    }

    public enum ErrorCode
    {
        Unknown,
    }
}
