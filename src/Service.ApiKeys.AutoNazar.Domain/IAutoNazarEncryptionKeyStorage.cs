using Service.ApiKeys.AutoNazar.Domain.Models.EncryptionKeys;
using System.Collections.Generic;

namespace Service.ApiKeys.AutoNazar.Domain
{
    public interface IAutoNazarEncryptionKeyStorage
    {
        void AddOrUpdateEncryptionKey(EncryptionKey encryptionKey);

        IReadOnlyCollection<string> GetIdsList();

        byte[] GetEncryptionKey(string id);
        bool RemoveEncryptionKey(string id);
    }
}