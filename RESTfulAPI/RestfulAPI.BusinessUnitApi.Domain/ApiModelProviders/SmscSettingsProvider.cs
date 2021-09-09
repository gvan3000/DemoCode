using System;
using System.Net;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.Common;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.SmscSettingsService;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    class SmscSettingsProvider : LoggingBase, ISmscSettingProvider
    {
        private readonly ITeleenaServiceUnitOfWork _teleenaServiceUnitOfWork;

        public SmscSettingsProvider(ITeleenaServiceUnitOfWork teleenaServiceUnitOfWork, IJsonRestApiLogger logger) : base(logger)
        {
            _teleenaServiceUnitOfWork = teleenaServiceUnitOfWork;
        }

        public async Task<ProviderOperationResult<object>> CreateOrUpdate(bool useOaBasedSmsRouting, Guid accountId, string user)
        {
            var contract = new CreateOrUpdateAccountSmscSettingsContract
            {
                UseOABasedSmsRouting = useOaBasedSmsRouting,
                AccountId = accountId,
                User = user
            };
            var result = await _teleenaServiceUnitOfWork.SmscSettingsService.CreateOrUpdateAsync(contract).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                return ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.InternalServerError,
                    nameof(contract), result.ErrorMessage);
            }
            return ProviderOperationResult<object>.AcceptedResult();
        }

    }
}
