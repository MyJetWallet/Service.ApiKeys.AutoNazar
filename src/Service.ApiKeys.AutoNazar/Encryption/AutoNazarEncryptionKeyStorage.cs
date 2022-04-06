using MyJetWallet.ApiSecurityManager.SymmetricEncryption;
using MyJetWallet.Sdk.Service;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Service.ApiKeys.AutoNazar.Encryption
{
    public class AutoNazarEncryptionKeyStorage
    {
        private readonly byte[] _sessionEncryptionKey;
        private readonly ConcurrentDictionary<string, string> _storage;
        private readonly ISymmetricEncryptionService _symmetricEncryptionService;

        public AutoNazarEncryptionKeyStorage(ISymmetricEncryptionService symmetricEncryptionService)
        {
            var sessionEncryptionKey = symmetricEncryptionService.GetSha256Hash(Guid.NewGuid().ToString());

            _sessionEncryptionKey = sessionEncryptionKey;
            _storage = new ConcurrentDictionary<string, string>();
            _symmetricEncryptionService = symmetricEncryptionService;
        }

        public void AddOrUpdateEncryptionKey(EncryptionKey encryptionKey)
        {
            var json = encryptionKey.ToJson();

            var encryptedKey = _symmetricEncryptionService.Encrypt(json, _sessionEncryptionKey);

            _storage.AddOrUpdate(encryptionKey.Id, encryptedKey, (x, y) => encryptedKey);
        }

        public EncryptionKey GetEncryptionKey(string id)
        {
            if (!_storage.TryGetValue(id, out var key))
                return null;

            string decrypted = _symmetricEncryptionService.Decrypt(key, _sessionEncryptionKey);

            var val = Newtonsoft.Json.JsonConvert.DeserializeObject<EncryptionKey>(decrypted);

            return val;
        }

        public IReadOnlyCollection<string> GetIdsList()
        {
            return _storage.Keys.ToArray();
        }

        public bool RemoveEncryptionKey(string id)
        {
            return _storage.Remove(id, out _);
        }
    }
}
