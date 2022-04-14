using Autofac;
using Microsoft.Extensions.Logging;
using MyJetWallet.ApiSecurityManager.Autofac;
using MyJetWallet.Sdk.Service;
using MyJetWallet.Sdk.Service.Tools;
using MyNoSqlServer.Abstractions;
using Polly;
using Polly.Retry;
using Service.ApiKeys.AutoNazar.Domain;
using Service.ApiKeys.AutoNazar.Domain.Impl;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;

namespace Service.ApiKeys.AutoNazar.Jobs
{
    public class AutoNazarJob : IStartable, IDisposable
    {
        private readonly ILogger<AutoNazarJob> _logger;
        private readonly MyTaskTimer _timer;
        private readonly IMyNoSqlServerDataReader<ApiKeyRecordNoSql> _reader;
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly IAutoNazarEncryptionKeyStorage _encryptionKeyStorage;
        private readonly IAutoNazarApiKeyStorage _autoNazarApiKeyStorage;

        public AutoNazarJob(
            ILogger<AutoNazarJob> logger,
            IMyNoSqlServerDataReader<ApiKeyRecordNoSql> reader,
            ITelegramBotClient telegramBotClient,
            IAutoNazarEncryptionKeyStorage encryptionKeyStorage,
            IAutoNazarApiKeyStorage autoNazarApiKeyStorage)
        {
            _logger = logger;
            _reader = reader;
            _telegramBotClient = telegramBotClient;
            _encryptionKeyStorage = encryptionKeyStorage;
            _autoNazarApiKeyStorage = autoNazarApiKeyStorage;
            _timer = new MyTaskTimer(typeof(AutoNazarJob),
                TimeSpan.FromSeconds(Program.Settings.CheckPeriodInSeconds),
                logger,
                DoProcess);
        }

        private async Task DoProcess()
        {
            _logger.LogInformation("Start auto nazar for all api keys");
            try
            {
                var all = _reader.Get();
                var allApiKeys = await _autoNazarApiKeyStorage.GetIdsList();

                var notSetEncKeys = all.Select(x => x.ApiKey.EncryptionKeyId).Where(x => !_encryptionKeyStorage.Contains(x)).Distinct();
                var notSetApiKeys = all.Select(x => x.ApiKey.ApiKeyId).Except(allApiKeys).Distinct();

                foreach (var item in notSetEncKeys)
                {
                    await _telegramBotClient.SendTextMessageAsync(Program.Settings.TelegramChatId,
                            $"AutoNazar has no encryption key! {item}!");
                }

                foreach (var item in notSetApiKeys)
                {
                    await _telegramBotClient.SendTextMessageAsync(Program.Settings.TelegramChatId,
                            $"AutoNazar has no ApiKey! {item}!");
                }

                foreach (var item in all)
                {
                    _logger.LogInformation("Checking for: {item}", item.ToJson());

                    var encKey = _encryptionKeyStorage.GetEncryptionKey(item.ApiKey.EncryptionKeyId);

                    if (encKey == null)
                    {
                        continue;
                    }

                    var apiKey = await _autoNazarApiKeyStorage.Get(item.ApiKey.ApiKeyId);

                    if (apiKey == null)
                    {
                        continue;
                    }

                    try
                    {
                        var factory = new ApiSecurityManagerClientFactory(item.ApiKey.ApplicationUri);
                        var apiKeyClient = factory.GetApiKeyService();

                        var response = await apiKeyClient.SetApiKeysAsync(
                            new MyJetWallet.ApiSecurityManager.Grpc.Models.SetApiKeyRequest
                            {
                                ApiKey = apiKey.ApiKeyValue,
                                ApiKeyId = apiKey.Id,
                                EncryptionKeyId = apiKey.EncryptionKeyId,
                                PrivateKey = apiKey.PrivateKeyValue,
                            });

                        if (response.Error != null)
                        {
                            _logger.LogError("When AutoNazarJob this error happened: {error}", response.Error.ToJson());
                        }
                        else
                        {
                            _logger.LogInformation("ApiKey is set for: {item}",
                                                    item.ToJson());
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "When AutoNazarJob this error happened. CONTINUE: {context}", item.ToJson());
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
