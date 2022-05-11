using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyJetWallet.ApiSecurityManager.Hashing;
using MyJetWallet.ApiSecurityManager.SymmetricEncryption;
using MyNoSqlServer.Abstractions;
using NUnit.Framework;
using Service.ApiKeys.AutoNazar.Domain;
using Service.ApiKeys.AutoNazar.Domain.Impl;
using Service.ApiKeys.AutoNazar.Domain.Models.ApiKeys;
using Service.ApiKeys.AutoNazar.Domain.Models.EncryptionKeys;
using Service.ApiKeys.AutoNazar.NoSql;
using System.Threading.Tasks;

namespace Service.ApiKeys.AutoNazar.Tests
{
    public class BaseFlowCheck
    {
        private const string LongEncKey = "gnASDhnwr134$gha%7asd#@";

        [Test]
        public async Task CheckBaseFlowTest()
        {
            var provider = CreateContainer();

            var encryptionKeyStorage = provider.Resolve<IAutoNazarEncryptionKeyStorage>();
            var apiKeyStorage = provider.Resolve<IAutoNazarApiKeyStorage>();
            var symmetricEncryptionService = provider.Resolve<ISymmetricEncryptionService>();
            const string testId = "test";
            const string apiId = "api";

            encryptionKeyStorage.AddOrUpdateEncryptionKey(new EncryptionKey
            {
                EncryptionKeyValue = "SomeEncryptionKeyForTest",
                Id = testId
            });

            await apiKeyStorage.AddOrUpdate(ApiKey.Create(
                apiId,
                "ApiKeyForSomeApi",
                "CheckWord",
                testId,
                "PrivateKeyForSomeApi",
                System.DateTime.UtcNow
            ));

            var encKey = encryptionKeyStorage.GetEncryptionKey(testId);
            var apiKey = await apiKeyStorage.Get(apiId);

            var hash = symmetricEncryptionService.GetSha256Hash("SomeEncryptionKeyForTest");

            for (int i = 0; i < encKey.Length; i++)
            {
                Assert.AreEqual(hash[i], encKey[i]);
            }

            Assert.AreEqual("ApiKeyForSomeApi", apiKey.ApiKeyValue);
            Assert.AreEqual("PrivateKeyForSomeApi", apiKey.PrivateKeyValue);
            Assert.AreEqual("CheckWord", apiKey.CheckWord);
        }

        private static IContainer CreateContainer()
        {
            var serviceCollection = new ServiceCollection();

            var container = new ContainerBuilder();

            var loggerFactory =
                LoggerFactory.Create(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.IncludeScopes = true;
                        options.SingleLine = true;
                        options.TimestampFormat = "hh:mm:ss ";
                    }));

            serviceCollection.AddSingleton(loggerFactory);
            serviceCollection.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            container.Populate(serviceCollection);

            container.RegisterType<InMemoryMyNoSqlServerDataWriter<ApiKeyNoSqlEntity>>()
                .As<IMyNoSqlServerDataWriter<ApiKeyNoSqlEntity>>()
                .SingleInstance();

            container
                .RegisterType<SymmetricEncryptionService>()
                .As<ISymmetricEncryptionService>()
                .SingleInstance();

            container
                .RegisterType<HashingService>()
                .AsSelf()
                .SingleInstance();

            container
               .RegisterType<AutoNazarEncryptionKeyStorage>()
               .As<IAutoNazarEncryptionKeyStorage>()
               .SingleInstance();

            container
               .RegisterType<AutoNazarApiKeyStorage>()
               .As<IAutoNazarApiKeyStorage>()
               .SingleInstance();

            var provider = container.Build();

            return provider;
        }

        [Test]
        public void CheckEncryptionForLargeKeys()
        {
            var provider = CreateContainer();

            var encryptionKeyStorage = provider.Resolve<IAutoNazarEncryptionKeyStorage>();
            var symmetricEncryptionService = provider.Resolve<ISymmetricEncryptionService>();
            const string testId = "test";

            var hash = symmetricEncryptionService.GetSha256Hash(LongEncKey);
            encryptionKeyStorage.AddOrUpdateEncryptionKey(new EncryptionKey
            {
                EncryptionKeyValue = LongEncKey,
                Id = testId
            });

            var encKey = encryptionKeyStorage.GetEncryptionKey(testId);

            for (int i = 0; i < encKey.Length; i++)
            {
                Assert.AreEqual(hash[i], encKey[i]);
            }
        }
    }
}