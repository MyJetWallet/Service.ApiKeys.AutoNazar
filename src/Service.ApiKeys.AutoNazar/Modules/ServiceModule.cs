using Autofac;
using MyJetWallet.ApiSecurityManager.Autofac;
using MyJetWallet.ApiSecurityManager.Hashing;
using MyJetWallet.ApiSecurityManager.SymmetricEncryption;
using MyJetWallet.Sdk.NoSql;
using Service.ApiKeys.AutoNazar.Domain;
using Service.ApiKeys.AutoNazar.Domain.Impl;
using Service.ApiKeys.AutoNazar.Jobs;
using Service.ApiKeys.AutoNazar.NoSql;
using Telegram.Bot;

namespace Service.ApiKeys.AutoNazar.Modules
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var telegramBot = new TelegramBotClient(Program.Settings.BotApiKey);
            var myNoSqlClient = builder.CreateNoSqlClient(Program.Settings.MyNoSqlReaderHostPort, Program.LogFactory);

            builder.RegisterMyNoSqlWriter<ApiKeyNoSqlEntity>(
                  () => Program.Settings.MyNoSqlWriterUrl,
                        ApiKeyNoSqlEntity.TableName);

            builder.RegisterInstance(telegramBot)
                .As<ITelegramBotClient>()
                .SingleInstance();

            builder.RegisterMyNoSqlReader<ApiKeyRecordNoSql>(myNoSqlClient, ApiKeyRecordNoSql.TableName);

            //builder.RegisterEncryptionServiceClient("auto-nazar", null);
            builder
            .RegisterType<HashingService>()
            .AsSelf()
            .SingleInstance();

            builder
               .RegisterType<AutoNazarJob>()
               .AsSelf()
               .AutoActivate()
               .SingleInstance();

            builder
               .RegisterType<AutoNazarEncryptionKeyStorage>()
               .As<IAutoNazarEncryptionKeyStorage>()
               .AutoActivate()
               .SingleInstance();

            builder
               .RegisterType<AutoNazarApiKeyStorage>()
               .As<IAutoNazarApiKeyStorage>()
               .AutoActivate()
               .SingleInstance();

            builder
                .RegisterType<SymmetricEncryptionService>()
                .As<ISymmetricEncryptionService>()
                .SingleInstance();
        }
    }
}