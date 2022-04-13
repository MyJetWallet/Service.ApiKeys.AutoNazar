using System.ServiceModel;
using System.Threading.Tasks;
using Service.ApiKeys.AutoNazar.Grpc.Models;

namespace Service.ApiKeys.AutoNazar.Grpc
{
    [ServiceContract]
    public interface IAutoNazarApiKeyGrpcService
    {
        [OperationContract]
        Task<SetApiKeyResponse> SetApiKeysAsync(SetApiKeyRequest request);

        [OperationContract]
        Task<GetApiKeyIdsResponse> GetApiKeyIdsAsync(GetApiKeyIdsRequest request);

        [OperationContract]
        Task<DeleteApiKeyResponse> DeleteApiKeyAsync(DeleteApiKeyRequest request);

        [OperationContract]
        Task<GetApiKeysResponse> GetApiKeysAsync(GetApiKeysRequest request);
    }
}