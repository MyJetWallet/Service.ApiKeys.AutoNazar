using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.Sdk.Service;
using System.Linq;
using Service.ApiKeys.AutoNazar.Grpc;
using Service.ApiKeys.AutoNazar.Grpc.Models;
using Service.ApiKeys.AutoNazar.Domain.Models.EncryptionKeys;
using Service.ApiKeys.AutoNazar.Domain.Impl;
using Service.ApiKeys.AutoNazar.Domain;
using System.Collections.Generic;

namespace MyJetWallet.ApiSecurityManager.Grpc.Services
{
    public class AutoNazarEncryptionKeyGrpcService : IAutoNazarEncryptionKeyGrpcService
    {
        private readonly IAutoNazarEncryptionKeyStorage _encryptionKeyStorage;
        private readonly IAutoNazarApiKeyStorage _apiKeyStorage;
        private readonly ILogger<AutoNazarEncryptionKeyGrpcService> _logger;

        public AutoNazarEncryptionKeyGrpcService(
            IAutoNazarEncryptionKeyStorage encryptionKeyStorage,
            IAutoNazarApiKeyStorage apiKeyStorage,
            ILogger<AutoNazarEncryptionKeyGrpcService> logger)
        {
            _encryptionKeyStorage = encryptionKeyStorage;
            _apiKeyStorage = apiKeyStorage;
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

            try
            {
                _encryptionKeyStorage.AddOrUpdateEncryptionKey(new EncryptionKey()
                {
                    ServiceName = request.ServiceName,
                    EncryptionKeyValue = request.EncryptionKey,
                    CheckWord = request.CheckWord,
                    Id = request.Id,
                });
                var apiKeys = await _apiKeyStorage.GetApiKeys();
                var list = new List<string>();
                var shoudResetEncKey = false;

                if (apiKeys.Any())
                {
                    foreach (var item in apiKeys)
                    {
                        if (item.EncryptionKeyId == request.Id)
                        {
                            var unprotected = await _apiKeyStorage.Get(item.Id);
                            if (unprotected.CheckWord?.Equals(request.CheckWord) ?? false)
                            {
                                list.Add(item.Id);
                            }
                            else
                            {
                                shoudResetEncKey = true;
                                break;
                            }
                        }
                    }
                }

                if (shoudResetEncKey)
                {
                    _encryptionKeyStorage.RemoveEncryptionKey(request.Id);
                    return new SetEncryptionKeyResponse
                    {
                        ActivatedIds = Array.Empty<string>(),
                    };
                }

                return new SetEncryptionKeyResponse
                {
                    ActivatedIds = list.ToArray(),
                };
            }
            catch (Exception e)
            {
                _logger?.LogError(e, "SetEncryptionKeyAsync Error: {context}",
                    request.Id);

                _encryptionKeyStorage.RemoveEncryptionKey(request.Id);

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
