using System;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.BusinessUnitApi.Domain.Validators;
using RestfulAPI.Common;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    public class APNProvider : LoggingBase, IAPNProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _translators;
        private readonly IBusinessUnitValidators _validators;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="logger">obligatory logger</param>
        /// <param name="serviceUnitOfWork">access to teleena wcf services</param>
        /// <param name="translators">collection of translators</param>
        /// <param name="validators">collection of validators</param>
        public APNProvider(IJsonRestApiLogger logger, ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators translators, IBusinessUnitValidators validators)
            : base(logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _translators = translators;
            _validators = validators;
        }
        /// <summary>
        /// Get APN sets by UserCompanyId
        /// </summary>
        /// <param name="userCompanyId">Id of the users set CompanyId</param>
        /// <returns></returns>
        public async Task<APNSetList> GetCompanyAPNsAsync(Guid userCompanyId)
        {
            var request = new GetApnSetsWithDetailsForCompanyRequestContract
            {
                CompanyId = userCompanyId
            };

            Logger.Log(LogSeverity.Debug, $"getting APNs for user company with id {userCompanyId}", nameof(GetCompanyAPNsAsync));

            var serviceResponse = await _serviceUnitOfWork.ApnService.GetApnSetsWithDetailsForCompanyAsync(request);
            if (serviceResponse == null)
            {
                Logger.Log(LogSeverity.Warning, $"{nameof(_serviceUnitOfWork.ApnService)}.{nameof(_serviceUnitOfWork.ApnService.GetApnSetsWithDetailsForCompanyAsync)} for CompanyId {userCompanyId} returned null response", nameof(GetCompanyAPNsAsync));
                return null;
            }

            Logger.Log(LogSeverity.Debug, $"got service response, number of apn sets: {serviceResponse.Length}", nameof(GetCompanyAPNsAsync));
            var result = _translators.APNTranslator.Translate(serviceResponse);
            return result;
        }

        public async Task<ProviderOperationResult<object>> UpdateBusinessUnitApnsAsync(Guid businessUnitId, Guid companyId, UpdateBusinessUnitApnsModel input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            var availableApns = await _serviceUnitOfWork.ApnService.GetApnSetsWithDetailsForCompanyAsync(new GetApnSetsWithDetailsForCompanyRequestContract() { CompanyId = companyId });

            var validationResult = _validators.UpdateBusinessUnitApnsValidator.ValidateModel(input, availableApns);
            if (!validationResult.IsSuccess)
                return validationResult;

            var serviceContract = _translators.UpdateApnsTranslator.Translate(input, availableApns);
            serviceContract.AccountId = businessUnitId;

            try
            {
                Logger.Log(LogSeverity.Info, $"Updating apns for business unit {businessUnitId}", nameof(UpdateBusinessUnitApnsAsync));
                await _serviceUnitOfWork.ApnService.SetApnDetailsForAccountAsync(serviceContract);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Failed to update apns for business unit {businessUnitId}", nameof(UpdateBusinessUnitApnsAsync), ex);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(input), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            Logger.Log(LogSeverity.Info, $"Finished updating apns for business unit {businessUnitId}", nameof(UpdateBusinessUnitApnsAsync));
            return ProviderOperationResult<object>.OkResult();
        }

        /// <summary>
        /// GetAPNsAsync
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<APNsResponseModel>> GetAPNsAsync(Guid businessUnitId)
        {
            Logger.Log(LogSeverity.Debug, $"Getting List of APNs for business unit with id {businessUnitId}", nameof(GetAPNsAsync));

            ApnDetailContract[] serviceResponse;
            try
            {
                serviceResponse = await _serviceUnitOfWork.ApnService.GetApnDetailsForAccountAsync(new GetApnDetailsForAccountRequestContract() { AccountId = businessUnitId });
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Service call to get APN details has faulted", nameof(GetAPNsAsync), ex);
                return ProviderOperationResult<APNsResponseModel>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(businessUnitId), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            if (serviceResponse == null)
            {
                Logger.Log(LogSeverity.Warning, $"{nameof(_serviceUnitOfWork.ApnService)}.{nameof(_serviceUnitOfWork.ApnService.GetApnDetailsForAccountAsync)} for business unit {businessUnitId} returned null response", nameof(GetAPNsAsync));
                return null;
            }

            var result = _translators.ApnsResponseModelTranslator.Translate(serviceResponse);

            return ProviderOperationResult<APNsResponseModel>.OkResult(result);
        }

        /// <summary>
        /// Update Business Unit Default Apn
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <param name="companyId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<object>> UpdateBusinessUnitDefaultApnAsync(Guid businessUnitId, Guid companyId, UpdateBusinessUnitDefaultApnModel input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if (input.Id == Guid.Empty)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(input.Id), $"Default APN id must be set.");
            }

            ApnDetailContract[] apnsDetailsForBU;
            try
            {
                apnsDetailsForBU = await _serviceUnitOfWork.ApnService.GetApnDetailsForAccountAsync(new GetApnDetailsForAccountRequestContract() { AccountId = businessUnitId });
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Service call to get APN details has faulted", nameof(UpdateBusinessUnitDefaultApnAsync), ex);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(businessUnitId), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            var validationResult = _validators.UpdateBusinessUnitDefaultApnValidator.ValidateModel(input, apnsDetailsForBU);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var serviceContract = _translators.UpdateDefaultApnTranslator.Translate(input, apnsDetailsForBU);
            serviceContract.AccountId = businessUnitId;

            string defaultApnPrefix = "Updating default apn";
            try
            {
                Logger.Log(LogSeverity.Info, $"{defaultApnPrefix} - updating apns for business unit {businessUnitId}", nameof(UpdateBusinessUnitDefaultApnAsync));
                await _serviceUnitOfWork.ApnService.SetApnDetailsForAccountAsync(serviceContract);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"{defaultApnPrefix} - Failed to update apns for business unit {businessUnitId}", nameof(UpdateBusinessUnitDefaultApnAsync), ex);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(input), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            Logger.Log(LogSeverity.Info, $"{defaultApnPrefix} - Finished updating apns for business unit {businessUnitId}", nameof(UpdateBusinessUnitDefaultApnAsync));
            return ProviderOperationResult<object>.OkResult();
        }

        /// <summary>
        /// Removes the link between the APN and the business unit
        /// </summary>
        /// <param name="businessUnitId">Id of the business unit</param>
        /// <param name="apnId">Apn Id</param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<object>> RemoveApnAsync(Guid businessUnitId, Guid apnId)
        {
            var apnsLinkedToAccount = await _serviceUnitOfWork.ApnService.GetApnDetailsForAccountAsync(new GetApnDetailsForAccountRequestContract() { AccountId = businessUnitId });

            var validationResult = _validators.DeleteApnValidator.Validate(apnsLinkedToAccount?.ToList(), apnId);

            if (!validationResult.IsSuccess)
                return validationResult;

            var request = _translators.DeleteApnTranslator.Translate(apnsLinkedToAccount, apnId, businessUnitId);

            try
            {
                await _serviceUnitOfWork.ApnService.SetApnDetailsForAccountAsync(request);
            }
            catch (FaultException<TeleenaInnerExp> exc)
            {
                Logger.LogException(LogSeverity.Error, $"Failed to remove apn: {apnId} for business unit id: {businessUnitId}", nameof(RemoveApnAsync), exc);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(exc.Detail.ErrorCode, nameof(businessUnitId), exc.Detail.ErrorDescription, exc.Detail.TicketId);
            }

            return ProviderOperationResult<object>.OkResult();
        }
    }
}
