using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.ApiSecurityManager.Autofac;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Polly;
using Polly.Retry;
using Service.ApiKeys.AutoNazar.Encryption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Service.ApiKeys.AutoNazar.Jobs
{
    public class AutoNazarJob : IStartable, IDisposable
    {
        private readonly ILogger<AutoNazarJob> _logger;
        private readonly MyTaskTimer _timer;
        private readonly AsyncRetryPolicy _retryPolicy;
        private readonly IMyNoSqlServerDataReader<ApiKeyRecordNoSql> _reader;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AutoNazarEncryptionKeyStorage _encryptionKeyStorage;

        public AutoNazarJob(
            ILogger<AutoNazarJob> logger,
            IMyNoSqlServerDataReader<ApiKeyRecordNoSql> reader,
            ITelegramBotClient telegramBotClient,
            AutoNazarEncryptionKeyStorage encryptionKeyStorage)
        {
            _logger = logger;
            _reader = reader;
            _telegramBotClient = telegramBotClient;
            _encryptionKeyStorage = encryptionKeyStorage;
            _timer = new MyTaskTimer(typeof(AutoNazarJob),
                TimeSpan.FromSeconds(Program.Settings.CheckPeriodInSeconds),
                logger,
                DoProcess);
            _retryPolicy = Policy
                          .Handle<Exception>()
                          .WaitAndRetryAsync(3, (i) => TimeSpan.FromMilliseconds(100 * (int)Math.Pow(2, i)));
        }

        private async Task DoProcess()
        {
            _logger.LogInformation("Start auto nazar for all api keys");
            try
            {
                var all = _reader.Get();

                foreach (var item in all)
                {
                    _logger.LogInformation("Checking for: {item}", item.ToJson());

                    var factory = new ApiSecurityManagerClientFactory(item.ApiKey.ApplicationUri);
                    var apiKeyClient = factory.GetApiKeyService();
                    var encryptionKeyGrpcService = factory.GetEncryptionKeyGrpcService();
                    var isApiKeySet = false;
                    var isEncryptionKeySet = false;

                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var apiKeys = await apiKeyClient.GetApiKeyIdsAsync(new MyJetWallet.ApiSecurityManager.Grpc.Models.GetApiKeyIdsRequest());

                        isApiKeySet = apiKeys?.Ids?.Any(x => x == item.ApiKey.ApiKeyId) ?? false;
                    });

                    await _retryPolicy.ExecuteAsync(async () =>
                    {
                        var encryptionKeys = await encryptionKeyGrpcService.GetEncryptionKeyIdsAsync(new MyJetWallet.ApiSecurityManager.Grpc.Models.GetEncryptionKeyIdsRequest());

                        isEncryptionKeySet = encryptionKeys?.Ids?.Any(x => x == item.ApiKey.EncryptionKeyId) ?? false;
                    });

                    _logger.LogInformation("Checking for: {item}, isApiKeySet: {isApiKeySet}, isEncryptionKeySet: {isEncryptionKeySet}",
                        item.ToJson(), isApiKeySet, isEncryptionKeySet);

                    if (isApiKeySet && !isEncryptionKeySet)
                    {
                        var encKey = _encryptionKeyStorage.GetEncryptionKey($"{item.ApiKey.ApplicationName}_{item.ApiKey.EncryptionKeyId}");

                        if (encKey == null)
                        {
                            await _telegramBotClient.SendTextMessageAsync(Program.Settings.TelegramChatId,
                                $"AutoNazar has no key! {item.ApiKey.ApplicationName} {item.ApiKey.EncryptionKeyId}!");
                            return;
                        }

                        var response = await encryptionKeyGrpcService.SetEncryptionKeyAsync(new MyJetWallet.ApiSecurityManager.Grpc.Models.SetEncryptionKeyRequest
                        {
                            CheckWord = encKey.CheckWord,
                            EncryptionKey = encKey.EncryptionKeyValue,
                            Id = item.ApiKey.EncryptionKeyId,
                        });

                        if (response.Error != null)
                        {
                            _logger.LogError("When AutoNazarJob this error happened: {error}", response.Error.ToJson());
                        }
                        else
                        {
                            _logger.LogInformation("Encryption is set for: {item}, isApiKeySet: {isApiKeySet}, isEncryptionKeySet: {isEncryptionKeySet}",
                        item.ToJson(), isApiKeySet, true);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When AutoNazarJob this error happened");
                throw;
            }

            _logger.LogInformation("AutoNazarJob has been completed");
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
