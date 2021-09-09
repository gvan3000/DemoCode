using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using System;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceProfileModels;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using System.ServiceModel;
using TeleenaLogging.Abstraction;


namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    public class BalanceProfileProvider : LoggingBase, IBalanceProfileProvider
    {
        private readonly ITeleenaServiceUnitOfWork _services;
        private readonly IBusinessUnitApiTranslators _translators;

        public BalanceProfileProvider(IJsonRestApiLogger logger, ITeleenaServiceUnitOfWork services, IBusinessUnitApiTranslators translators)
            :base(logger)
        {
            _services = services;
            _translators = translators;
        }

        public async Task<ProviderOperationResult<BalanceProfileListModel>> GetBalanceProfilesAsync(Guid businessUnitId)
        {
            if (businessUnitId == Guid.Empty)
                throw new ArgumentException("Can't be empty guid", nameof(businessUnitId));

            var account = await _services.AccountService.GetAccountByIdAsync(businessUnitId);
            if (account == null)
            {
                Logger.Log(LogSeverity.Error, "Error getting account contract", nameof(GetBalanceProfilesAsync));
                return ProviderOperationResult<BalanceProfileListModel>.NotFoundResult($"Could not find business unit with id of {businessUnitId}");
            }

            var serviceRequest = new GetBalanceProfilesPerCompanyContract()
            {
                CompanyId = account.CompanyId.GetValueOrDefault()
            };

            SysCodeContract[] serviceResponse;
            try
            {
                serviceResponse = await _services.BalanceService.GetBalanceProfilesPerCompanyAsync(serviceRequest);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Service call to get balance profiles has faulted", nameof(GetBalanceProfilesAsync), ex);
                return ProviderOperationResult<BalanceProfileListModel>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(businessUnitId), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            if (serviceResponse == null)
            {
                Logger.Log(LogSeverity.Warning, $"No balance profiles returned from service for company id {serviceRequest.CompanyId}", nameof(GetBalanceProfilesAsync));
            }

            var result = _translators.BalanceProfileListTranslator.Translate(serviceResponse);

            return ProviderOperationResult<BalanceProfileListModel>.OkResult(result);
        }
    }
}
