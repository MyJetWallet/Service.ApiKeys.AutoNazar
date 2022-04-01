using System.ServiceModel;
using System.Threading.Tasks;
using Service.ApiKeys.AutoNazar.Grpc.Models;

namespace Service.ApiKeys.AutoNazar.Grpc
{
    [ServiceContract]
    public interface IAutoNazarEncryptionKeyGrpcService
    {
        [OperationContract]
        Task<GetEncryptionKeyIdsResponse> GetEncryptionKeyIdsAsync(GetEncryptionKeyIdsRequest request);

        [OperationContract]
        Task<SetEncryptionKeyResponse> SetEncryptionKeyAsync(SetEncryptionKeyRequest request);
    }
}