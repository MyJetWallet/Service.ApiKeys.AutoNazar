using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using System.Linq;
using Service.ApiKeys.AutoNazar.Grpc;
using Service.ApiKeys.AutoNazar.Grpc.Models;
using Service.ApiKeys.AutoNazar.Encryption;

namespace MyJetWallet.ApiSecurityManager.Grpc.Services
{
    public class AutoNazarEncryptionKeyGrpcService : IAutoNazarEncryptionKeyGrpcService
    {
        private readonly AutoNazarEncryptionKeyStorage _encryptionKeyStorage;
        private readonly ILogger<AutoNazarEncryptionKeyGrpcService> _logger;

        public AutoNazarEncryptionKeyGrpcService(
            AutoNazarEncryptionKeyStorage encryptionKeyStorage,
            ILogger<AutoNazarEncryptionKeyGrpcService> logger)
        {
            _encryptionKeyStorage = encryptionKeyStorage;
            _logger = logger;
        }

        public Task<GetEncryptionKeyIdsResponse> GetEncryptionKeyIdsAsync(GetEncryptionKeyIdsRequest request)
        {
            _logger.LogInformation("GetEncryptionKeyIdsAsync {context}", request.ToJson());
            try
            {
                var ids = _encryptionKeyStorage.GetIdsList();

                return Task.FromResult(new GetEncryptionKeyIdsResponse
                {
                    Ids = ids
                });
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "GetEncryptionKeyIdsAsync Error: {context}",
                    request.ToJson());
                return Task.FromResult(new GetEncryptionKeyIdsResponse
                {
                    Error = new ErrorResponse
                    {
                        Code = ErrorCode.Unknown,
                        Message = e.Message,
                    }
                });
            }

        }

        public async Task<SetEncryptionKeyResponse> SetEncryptionKeyAsync(SetEncryptionKeyRequest request)
        {
            _logger.LogInformation("SetEncryptionKeyResponse {context}", request.Id);

            var autoNazarId = $"{request.ServiceName}_{request.Id}";

            try
            {
                _encryptionKeyStorage.AddOrUpdateEncryptionKey(new EncryptionKey()
                {
                    ServiceName = request.ServiceName,
                    EncryptionKeyValue = request.EncryptionKey,
                    CheckWord = request.CheckWord,
                    Id = autoNazarId,
                });

                var factory = new Autofac.ApiSecurityManagerClientFactory(request.ServiceUri);
                var encryptionKeyClient = factory.GetEncryptionKeyGrpcService();

                var response = await encryptionKeyClient.SetEncryptionKeyAsync(new()
                {
                    EncryptionKey = request.EncryptionKey,
                    Id = request.Id,
                    CheckWord = request.CheckWord,
                });

                if (response.Error != null)
                {
                    _logger?.LogError("SetEncryptionKeyAsync Error: {context}",
                    (new {request.Id, response.Error}).ToJson());
                    _encryptionKeyStorage.RemoveEncryptionKey(autoNazarId);

                    return new SetEncryptionKeyResponse
                    {
                        ActivatedIds = Array.Empty<string>(),
                    };
                }

                if (response.ActivatedIds?.Any() ?? false)
                {
                    return new SetEncryptionKeyResponse
                    {
                        ActivatedIds = response.ActivatedIds,
                    };
                }

                return new SetEncryptionKeyResponse
                {
                    ActivatedIds = Array.Empty<string>(),
                };
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "SetEncryptionKeyAsync Error: {context}",
                    request.Id);

                _encryptionKeyStorage.RemoveEncryptionKey(autoNazarId);

                return new SetEncryptionKeyResponse
                {
                    Error = new ErrorResponse
                    {
                        Code = ErrorCode.Unknown,
                        Message = e.Message,
                    }
                };
            }
        }
    }
}
