using System;
using System.Threading.Tasks;
using ProtoBuf.Grpc.Client;
using Service.ApiKeys.AutoNazar.Client;
using Service.ApiKeys.AutoNazar.Grpc.Models;

namespace TestApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GrpcClientFactory.AllowUnencryptedHttp2 = true;

            Console.Write("Press enter to start");
            Console.ReadLine();


            var factory = new AutoNazarClientFactory("http://localhost:5001");
            var encClient = factory.GetEncryptionKeyGrpcService();
            var apiKeyClient = factory.GetApiKeyGrpcService();

            await encClient.SetEncryptionKeyAsync(new SetEncryptionKeyRequest { 
                Id = "wallet-api-encryption-key", 
                CheckWord = "CheckWord",
                EncryptionKey = "e537d941-f7d2-4939-b97b-ae4722ca56aa",
                ServiceName = "Wallet.Api.Auth",
                ServiceUri = "http://wallet-api-auth.spot-services.svc.cluster.local",
            });

            await encClient.SetEncryptionKeyAsync(new SetEncryptionKeyRequest
            {
                Id = "wallet-api-encryption-key",
                CheckWord = "CheckWord",
                EncryptionKey = "",
                ServiceName = "Wallet.Api.Auth",
                ServiceUri = "",
            });

            await apiKeyClient.SetApiKeysAsync(new SetApiKeyRequest
            {
                ApiKey = "",
                ApiKeyId = "wallet-api-encryption-key",
                ApplicationUri = "",
                CheckWord = "CheckWord",
                EncryptionKeyId= "wallet-api-encryption-key",
                PrivateKey = "",
            });
            ////
            ////var resp = await  client.SayHelloAsync(new HelloRequest(){Name = "Alex"});
            //Console.WriteLine(resp?.Message);

            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
