using MyJetWallet.ApiSecurityManager.SymmetricEncryption;
using MyJetWallet.Sdk.Service;
using Service.ApiKeys.AutoNazar.Domain.Models.EncryptionKeys;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Service.ApiKeys.AutoNazar.Domain.Impl
{
    public class AutoNazarEncryptionKeyStorage : IAutoNazarEncryptionKeyStorage
    {
        private readonly byte[] _sessionEncryptionKey;
        private readonly ConcurrentDictionary<string, byte[]> _storage;
        private readonly ISymmetricEncryptionService _symmetricEncryptionService;

        public AutoNazarEncryptionKeyStorage(ISymmetricEncryptionService symmetricEncryptionService)
        {
            var sessionEncryptionKey = symmetricEncryptionService.GetSha256Hash(Guid.NewGuid().ToString());

            _sessionEncryptionKey = sessionEncryptionKey;
            _storage = new ConcurrentDictionary<string, byte[]>();
            _symmetricEncryptionService = symmetricEncryptionService;
        }

        public void AddOrUpdateEncryptionKey(EncryptionKey encryptionKey)
        {
            var sha = _symmetricEncryptionService.GetSha256Hash(encryptionKey.EncryptionKeyValue);
            var encryptedKey = _symmetricEncryptionService.Encrypt(sha, _sessionEncryptionKey);

            _storage.AddOrUpdate(encryptionKey.Id, encryptedKey, (x, y) => encryptedKey);
        }

        public bool Contains(string id)
        {
            return _storage.ContainsKey(id);
        }

        public byte[] GetEncryptionKey(string id)
        {
            if (!_storage.TryGetValue(id, out var key))
                return null;

            var decrypted = _symmetricEncryptionService.Decrypt(key, _sessionEncryptionKey);

            return decrypted;
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
