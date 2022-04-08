using Autofac;
using Service.ApiKeys.AutoNazar.Grpc;

// ReSharper disable UnusedMember.Global

namespace Service.ApiKeys.AutoNazar.Client
{
    public static class AutofacHelper
    {
        public static void RegisterAutoNazarClient(this ContainerBuilder builder, string grpcServiceUrl)
        {
            var factory = new AutoNazarClientFactory(grpcServiceUrl);

            builder.RegisterInstance(factory.GetEncryptionKeyGrpcService())
                .As<IAutoNazarEncryptionKeyGrpcService>()
                .SingleInstance();

            builder.RegisterInstance(factory.GetApiKeyGrpcService())
                .As<IAutoNazarApiKeyGrpcService>()
                .SingleInstance();
        }
    }
}
