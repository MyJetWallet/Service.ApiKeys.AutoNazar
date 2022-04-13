using Service.ApiKeys.AutoNazar.Domain.Models.ApiKeys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.ApiKeys.AutoNazar.Domain
{
    public interface IAutoNazarApiKeyStorage
    {
        Task AddOrUpdate(ApiKey apiKey);

        Task<IReadOnlyCollection<string>> GetIdsList();

        Task<IReadOnlyCollection<ApiKeyId>> GetApiKeys();

        Task<ApiKey> Get(string id);
        Task Remove(string id);
    }
}