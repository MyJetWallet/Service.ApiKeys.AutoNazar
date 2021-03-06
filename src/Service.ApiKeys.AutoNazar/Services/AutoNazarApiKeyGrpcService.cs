using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyJetWallet.ApiSecurityManager.Autofac;
using MyJetWallet.Sdk.Service;
using Service.ApiKeys.AutoNazar.Domain;
using Service.ApiKeys.AutoNazar.Domain.Models.ApiKeys;
using Service.ApiKeys.AutoNazar.Grpc;
using Service.ApiKeys.AutoNazar.Grpc.Models;

namespace MyJetWallet.ApiSecurityManager.Grpc.Services
{
    public class AutoNazarApiKeyGrpcService : IAutoNazarApiKeyGrpcService
    {
        private readonly IAutoNazarApiKeyStorage _apiKeyStorage;
        private readonly ILogger<AutoNazarApiKeyGrpcService> _logger;

        public AutoNazarApiKeyGrpcService(IAutoNazarApiKeyStorage apiKeyStorage,
            ILogger<AutoNazarApiKeyGrpcService> logger)
        {
            _apiKeyStorage = apiKeyStorage;
            _logger = logger;
        }

        public async Task<GetApiKeyIdsResponse> GetApiKeyIdsAsync(GetApiKeyIdsRequest request)
        {
            try
            {
                var list = await _apiKeyStorage.GetIdsList();

                return new GetApiKeyIdsResponse
                {
                    Ids = list
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetApiKeyIdsAsync");

                return new GetApiKeyIdsResponse
                {
                    Error = new ErrorResponse
                    {
                        Code = ErrorCode.Unknown,
                        Message = e.Message,
                    }
                };
            }

        }

        public async Task<GetApiKeysResponse> GetApiKeysAsync(GetApiKeysRequest request)
        {
            try
            {
                var list = await _apiKeyStorage.GetApiKeys();

                return new GetApiKeysResponse
                {
                    Ids = list
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error GetApiKeysAsync");

                return new GetApiKeysResponse
                {
                    Error = new ErrorResponse
                    {
                        Code = ErrorCode.Unknown,
                        Message = e.Message,
                    }
                };
            }

        }

        //TODO: ADD LOGGING
        public async Task<SetApiKeyResponse> SetApiKeysAsync(SetApiKeyRequest request)
        {
            _logger.LogInformation("Set Api Keys {context}", (new
            {
                request.ApiKeyId,
                request.EncryptionKeyId,
            }).ToJson());

            var privateKey = request.PrivateKey?.Replace("-----BEGIN PRIVATE KEY-----", "");
            privateKey = privateKey?.Replace("-----END PRIVATE KEY-----", "");

            try
            {
                //var factory = new ApiSecurityManagerClientFactory(request.ApplicationUri);
                //var apiKeyService = factory.GetApiKeyService();

                //var response = await apiKeyService.SetApiKeysAsync(new ()
                //{
                //    ApiKey = request.ApiKey,
                //    ApiKeyId = request.ApiKeyId,
                //    EncryptionKeyId = request.EncryptionKeyId,
                //    PrivateKey = privateKey,
                //});

                //if (response.Error != null)
                //{
                //    throw new Exception($"Grpc Error from application: {request.ApplicationUri} {response.Error.Code}:{response.Error.Message}");
                //}

                await _apiKeyStorage.AddOrUpdate(ApiKey.Create(
                    request.ApiKeyId,
                    request.ApiKey,
                    request.CheckWord,
                    request.EncryptionKeyId,
                    privateKey,
                    DateTime.UtcNow));

                return new SetApiKeyResponse { };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Set Api Keys {context}", (new
                {
                    request.ApiKeyId,
                    request.EncryptionKeyId,
                }).ToJson());
                return new SetApiKeyResponse
                {
                    Error = new ErrorResponse
                    {
                        Code = ErrorCode.Unknown,
                        Message = e.Message,
                    }
                };
            }
        }

        public async Task<DeleteApiKeyResponse> DeleteApiKeyAsync(DeleteApiKeyRequest request)
        {
            _logger.LogInformation("Delete Api Keys {context}", (new
            {
                request.ApiKeyId,
            }).ToJson());

            try
            {
                await _apiKeyStorage.Remove(request.ApiKeyId);

                return new DeleteApiKeyResponse { };
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error Delete Api Keys {context}", (new
                {
                    request.ApiKeyId,
                }).ToJson());

                return new DeleteApiKeyResponse
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
