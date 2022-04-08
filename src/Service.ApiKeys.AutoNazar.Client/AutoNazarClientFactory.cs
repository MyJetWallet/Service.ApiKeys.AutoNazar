using JetBrains.Annotations;
using MyJetWallet.Sdk.Grpc;
using Service.ApiKeys.AutoNazar.Grpc;

namespace Service.ApiKeys.AutoNazar.Client
{
    [UsedImplicitly]
    public class AutoNazarClientFactory: MyGrpcClientFactory
    {
        public AutoNazarClientFactory(string grpcServiceUrl) : base(grpcServiceUrl)
        {
        }

        public IAutoNazarApiKeyGrpcService GetApiKeyGrpcService() => CreateGrpcService<IAutoNazarApiKeyGrpcService>();

        public IAutoNazarEncryptionKeyGrpcService GetEncryptionKeyGrpcService() => CreateGrpcService<IAutoNazarEncryptionKeyGrpcService>();
    }
}
