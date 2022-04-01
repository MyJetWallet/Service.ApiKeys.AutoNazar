using Autofac;
using MyJetWallet.ApiSecurityManager.Autofac;
using MyJetWallet.Sdk.NoSql;
using Service.ApiKeys.AutoNazar.Encryption;
using Service.ApiKeys.AutoNazar.Jobs;
using Telegram.Bot;

namespace Service.ApiKeys.AutoNazar.Modules
{
    public class ServiceModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var telegramBot = new TelegramBotClient(Program.Settings.BotApiKey);
            var myNoSqlClient = builder.CreateNoSqlClient(() => Program.Settings.MyNoSqlReaderHostPort);

            builder.RegisterInstance(telegramBot)
                .As<ITelegramBotClient>()
                .SingleInstance();

            builder.RegisterMyNoSqlReader<ApiKeyRecordNoSql>(myNoSqlClient, ApiKeyRecordNoSql.TableName);

            builder.RegisterEncryptionServiceClient("auto-nazar", null);

            builder
               .RegisterType<AutoNazarJob>()
               .AsSelf()
               .AutoActivate()
               .SingleInstance();

            builder
               .RegisterType<AutoNazarEncryptionKeyStorage>()
               .AsSelf()
               .AutoActivate()
               .SingleInstance();
        }
    }
}