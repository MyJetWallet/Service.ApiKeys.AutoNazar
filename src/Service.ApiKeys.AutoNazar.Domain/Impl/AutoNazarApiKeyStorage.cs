using MyNoSqlServer.Abstractions;
using Service.ApiKeys.AutoNazar.Domain.Models.ApiKeys;
using Service.ApiKeys.AutoNazar.NoSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Service.ApiKeys.AutoNazar.Domain.Impl
{
    public class AutoNazarApiKeyStorage : IAutoNazarApiKeyStorage
    {
        private readonly MyJetWallet.ApiSecurityManager.SymmetricEncryption.ISymmetricEncryptionService _symmetricEncryptionService;
        private readonly IAutoNazarEncryptionKeyStorage _encryptionKeyStorage;
        private readonly IMyNoSqlServerDataWriter<ApiKeyNoSqlEntity> _writer;

        public AutoNazarApiKeyStorage(
            MyJetWallet.ApiSecurityManager.SymmetricEncryption.ISymmetricEncryptionService symmetricEncryptionService,
            IAutoNazarEncryptionKeyStorage encryptionKeyStorage,
            IMyNoSqlServerDataWriter<ApiKeyNoSqlEntity> writer
            )
        {
            _symmetricEncryptionService = symmetricEncryptionService;
            _encryptionKeyStorage = encryptionKeyStorage;
            _writer = writer;
        }

        public async Task AddOrUpdate(ApiKey apiKey)
        {
            var encryptionKey = _encryptionKeyStorage.GetEncryptionKey(apiKey.EncryptionKeyId);

            if (encryptionKey == null)
                return;

            var securedApiKey = _symmetricEncryptionService.Encrypt(apiKey.ApiKeyValue, encryptionKey);
            var securedPrivateKey = string.IsNullOrEmpty(apiKey.PrivateKeyValue) ? null :
                _symmetricEncryptionService.Encrypt(apiKey.PrivateKeyValue, encryptionKey);
            var checkWord = _symmetricEncryptionService.Encrypt(apiKey.CheckWord, encryptionKey);

            apiKey.ApiKeyValue = securedApiKey;
            apiKey.PrivateKeyValue = securedPrivateKey;
            apiKey.CheckWord = checkWord;

            await _writer.InsertOrReplaceAsync(ApiKeyNoSqlEntity.Create(apiKey));
        }

        public async Task<ApiKey> Get(string id)
        {
            var key = await _writer.GetAsync(ApiKeyNoSqlEntity.GeneratePartitionKey(), ApiKeyNoSqlEntity.GenerateRowKey(id));

            if (key == null)
                return null;

            var encryptionKey = _encryptionKeyStorage.GetEncryptionKey(key.ApiKey.EncryptionKeyId);

            if (encryptionKey == null)
                return null;

            var decryptedApiKey = _symmetricEncryptionService.Decrypt(
                key.ApiKey.ApiKeyValue, encryptionKey);

            var decryptedPrivateKey = string.IsNullOrEmpty(key.ApiKey.PrivateKeyValue) ? null :
                _symmetricEncryptionService.Decrypt(key.ApiKey.PrivateKeyValue, encryptionKey);

            var decryptedCheckWord = string.IsNullOrEmpty(key.ApiKey.CheckWord) ? null :
                _symmetricEncryptionService.Decrypt(key.ApiKey.CheckWord, encryptionKey);

            return ApiKey.Create(id, decryptedApiKey, decryptedCheckWord,
                key.ApiKey.EncryptionKeyId, decryptedPrivateKey, key.ApiKey.RegisterDate);
        }

        public async Task<IReadOnlyCollection<ApiKey>> GetApiKeys()
        {
            var all = await _writer.GetAsync(ApiKeyNoSqlEntity.GeneratePartitionKey());

            return all.Select(x => x.ApiKey).ToArray();
        }

        public async Task<IReadOnlyCollection<string>> GetIdsList()
        {
            var all = await _writer.GetAsync(ApiKeyNoSqlEntity.GeneratePartitionKey());

            return all?.Select(x => x.ApiKey.Id).ToArray();
        }

        public async Task Remove(string id)
        {
            await _writer.DeleteAsync(ApiKeyNoSqlEntity.GeneratePartitionKey(), ApiKeyNoSqlEntity.GenerateRowKey(id));
        }
    }
}
