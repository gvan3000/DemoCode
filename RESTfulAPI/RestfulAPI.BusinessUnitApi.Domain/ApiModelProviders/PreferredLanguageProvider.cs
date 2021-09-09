using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.PreferredLanguageModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.BusinessUnitApi.Domain.Validators;
using RestfulAPI.BusinessUnitApi.Domain.Validators.PreferredLanguages;
using RestfulAPI.Common;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.PreferredLanguageService;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    public class PreferredLanguageProvider : LoggingBase, IPreferredLanguageProvider
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
        public PreferredLanguageProvider(IJsonRestApiLogger logger, ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators translators, IBusinessUnitValidators validators)
            : base(logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _translators = translators;
            _validators = validators;
        }

        /// <summary>
        /// Get available languages for business unit by company id
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<AvailableLanguagesResponseModel>> GetAvailableLanguagesAsync(Guid businessUnitId, Guid companyId)
        {
            Logger.Log(LogSeverity.Debug, $"Getting List of available languages for business unit with id {businessUnitId} by company Id: {companyId}", nameof(GetAvailableLanguagesAsync));

            GetCompanyLanguageContract[] serviceResponse;

            try
            {
                serviceResponse = await _serviceUnitOfWork.PreferredLanguageService.GetAvailableAccountLanguagesAsync(companyId);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Service call to get available languages of the busines unit Id: {businessUnitId} by company Id: {companyId} has faulted", nameof(GetAvailableLanguagesAsync), ex);
                return ProviderOperationResult<AvailableLanguagesResponseModel>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(businessUnitId), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            if (serviceResponse == null)
            {
                Logger.Log(LogSeverity.Warning, $"{nameof(_serviceUnitOfWork.PreferredLanguageService)}.{nameof(_serviceUnitOfWork.PreferredLanguageService.GetAvailableAccountLanguagesAsync)} for business unit {businessUnitId} returned null response", nameof(GetAvailableLanguagesAsync));
                return ProviderOperationResult<AvailableLanguagesResponseModel>.GenerateFailureResult(HttpStatusCode.InternalServerError, nameof(businessUnitId), "Internal server error. Please contact administrator.");
            }

            var result = _translators.AvailableLanguagesModelTranslator.Translate(serviceResponse);

            return ProviderOperationResult<AvailableLanguagesResponseModel>.OkResult(result);
        }

        /// <summary>
        /// Get business unit preferred languages by business unit id
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<PreferredLanguageResponseModel>> GetAccountLanguagesAsync(Guid businessUnitId)
        {
            Logger.Log(LogSeverity.Debug, $"Getting List of preferred languages for business unit with id {businessUnitId}", nameof(GetAccountLanguagesAsync));

            GetAccountLanguageContract[] serviceResponse;

            try
            {
                serviceResponse = await _serviceUnitOfWork.PreferredLanguageService.GetAccountLanguagesAsync(businessUnitId);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Service call to get preferred languages of the busines unit Id: {businessUnitId} has faulted", nameof(GetAccountLanguagesAsync), ex);
                return ProviderOperationResult<PreferredLanguageResponseModel>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, "PreferredLanguages", ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            if (serviceResponse == null)
            {
                Logger.Log(LogSeverity.Warning, $"{nameof(_serviceUnitOfWork.PreferredLanguageService)}.{nameof(_serviceUnitOfWork.PreferredLanguageService.GetAccountLanguagesAsync)} for business unit {businessUnitId} returned null response", nameof(GetAccountLanguagesAsync));
                return ProviderOperationResult<PreferredLanguageResponseModel>.GenerateFailureResult(HttpStatusCode.InternalServerError, nameof(businessUnitId), "Internal server error. Please contact administrator.");
            }

            var result = _translators.PreferredLanguageModelTranslator.Translate(serviceResponse);

            return ProviderOperationResult<PreferredLanguageResponseModel>.OkResult(result);
        }

        /// <summary>
        /// Update business unit preferred languages
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <param name="companyId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<object>> UpdateAccountLanguagesAsync(Guid businessUnitId, Guid companyId, UpdatePreferredLanguagesRequestModel input)
        {
            Logger.Log(LogSeverity.Info, $"Updating preferred languages for business unit {businessUnitId}", nameof(UpdateAccountLanguagesAsync));

            try
            {
                var validationResult = _validators.UpdatePreferredLanguagesValidator.ValidateModel(input);

                if (!validationResult.IsSuccess)
                    return validationResult;

                var request = _translators.UpdatePreferredLanguageContractTranslator.Translate(input);
                request.AccountId = businessUnitId;

                await _serviceUnitOfWork.PreferredLanguageService.UpdateAccountLanguagesAsync(request);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            { 
                Logger.LogException(LogSeverity.Error, $"Failed to update preferred languages for business unit {businessUnitId}", nameof(UpdateAccountLanguagesAsync), ex);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, "PreferredLanguages", ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            Logger.Log(LogSeverity.Info, $"Finished updating preferred languages for business unit {businessUnitId}", nameof(UpdateAccountLanguagesAsync));
            return ProviderOperationResult<object>.OkResult();
        }
    }
}
