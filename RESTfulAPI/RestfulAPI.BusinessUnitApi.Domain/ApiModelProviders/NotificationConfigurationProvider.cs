using System;
using System.ServiceModel;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.NotificationModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.BusinessUnitApi.Domain.Validators;
using RestfulAPI.Common;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.Constants;
using RestfulAPI.TeleenaServiceReferences.NotificationConfigurationService;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    /// <summary>
    /// Business unit notification configuration provider 
    /// </summary>
    public class NotificationConfigurationProvider : LoggingBase, INotificationConfigurationProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _translators;
        private readonly IBusinessUnitValidators _validators;

        /// <summary>
        /// Initialize NotificationConfigurationProvider
        /// </summary>
        /// <param name="logger">logger</param>
        /// <param name="serviceUnitOfWork">teleena wcf service unit of work</param>
        /// <param name="businessUnitApiTranslators">business unit translators</param>
        /// <param name="validators">Business unit validators</param>
        public NotificationConfigurationProvider(IJsonRestApiLogger logger,
                                                 ITeleenaServiceUnitOfWork serviceUnitOfWork,
                                                 IBusinessUnitApiTranslators businessUnitApiTranslators,
                                                 IBusinessUnitValidators validators)
            : base(logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _translators = businessUnitApiTranslators;
            _validators = validators;
        }

        /// <summary>
        /// Get business unit notification configuration
        /// </summary>
        /// <param name="businessUnitId">Business unit id</param>
        /// <returns><see cref="GetNotificationListDataModel"/></returns>
        public async Task<ProviderOperationResult<GetNotificationListDataModel>> GetBusinessUnitNotificationsAsync(Guid businessUnitId)
        {
            GetNotificationConfigurationListResponse response;
            try
            {
                response = await _serviceUnitOfWork.NotificationConfigurationService.GetBusinessUnitNotificationConfigurationAsync(businessUnitId);
            }
            catch (FaultException<TeleenaInnerExp> exc)
            {
                Logger.LogException(LogSeverity.Error, "Error while getting business unit notification configuration", nameof(GetBusinessUnitNotificationsAsync), exc);
                return ProviderOperationResult<GetNotificationListDataModel>
                    .TeleenaExceptionAsResult(exc.Detail.ErrorCode, nameof(businessUnitId), exc.Detail.ErrorDescription, exc.Detail.TicketId);
            }

            if (!response.Success)
            {
                Logger.Log(LogSeverity.Error, "Get business unit configuration notification failed, errors: {response.ErrorMessage}", nameof(GetBusinessUnitNotificationsAsync));
                return ProviderOperationResult<GetNotificationListDataModel>
                    .GenerateFailureResult(System.Net.HttpStatusCode.BadRequest, nameof(businessUnitId), response.ErrorMessage);
            }

            var translatedNotifications = _translators.NotificationListDataModelTranslator.Translate(response);

            return ProviderOperationResult<GetNotificationListDataModel>.OkResult(translatedNotifications);
        }

        public async Task<ProviderOperationResult<object>> UpdateBusinessUnitNotificationAsync(UpdateNotificationModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var requestValidation = _validators.UpdateBusinessUnitNotificationValidator.ValidateModel(model);
            if (!requestValidation.IsSuccess)
            {
                return requestValidation;
            }

            UpdateBusinessUnitNotificationConfigurationContract serviceRequest = _translators.UpdateNotificationConfigurationTranslator.Translate(model);

            UpdateNotificationConfigurationResultContract serviceResponse;

            try
            {
                serviceResponse = await _serviceUnitOfWork.NotificationConfigurationService.UpdateBusinessUnitNotificationConfigurationAsync(serviceRequest);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Error calling Teleena WCF service", nameof(UpdateBusinessUnitNotificationAsync), ex);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(model), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            if (serviceResponse == null)
            {
                Logger.Log(LogSeverity.Error, "Unexpected null returned from service call", nameof(UpdateBusinessUnitNotificationAsync));
                throw new InvalidOperationException("Unexpected null returned from service method when result is expected");
            }

            if (!serviceResponse.IsSuccess)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(model), serviceResponse.ErrorMessage);
            }

            return ProviderOperationResult<object>.AcceptedResult();
        }

        public async Task<ProviderOperationResult<CreateNotificationModelResponse>> CreateNotificationAsync(Guid businessUnitId, CreateNotificationModel input)
        {
            var validationResult = _validators.CreateBusinessUnitNotificationValidator.ValidateModel(input);
            if (!validationResult.IsSuccess)
                return validationResult;

            var createNotificationContract = _translators.CreateNotificationTranslator.Translate(input);
            createNotificationContract.AccountId = businessUnitId;

            try
            {
                var result = await _serviceUnitOfWork.NotificationConfigurationService.CreateBusinessUnitNotificationConfigurationAsync(createNotificationContract);

                if (!result.CreationSucceeded)
                {
                    Logger.Log(LogSeverity.Error, $"Failed to create business unit level notification configuration: {result.ErrorMessage}", nameof(CreateNotificationAsync));
                    return ProviderOperationResult<CreateNotificationModelResponse>.GenerateFailureResult(System.Net.HttpStatusCode.InternalServerError, nameof(input), result.ErrorMessage);
                }

                var translatedResult = _translators.CreateNotificationResponseTranslator.Translate(result);
                return ProviderOperationResult<CreateNotificationModelResponse>.OkResult(translatedResult);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Exception while executing service method to create business unit level notification configuration", nameof(CreateNotificationAsync), ex);
                return ProviderOperationResult<CreateNotificationModelResponse>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(input), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }
        }

        /// <summary>
        /// Delete business unit notification configuration
        /// </summary>
        /// <param name="businessUnitId">Id of the business unit</param>
        /// <param name="notificationType">Notification type</param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<object>> DeleteBusinessUnitNotificationAsync(Guid businessUnitId, NotificationType notificationType)
        {
            DeleteNotificationConfigurationResponse response;

            string processId = _translators.NotificationTypeTranslator.Translate(notificationType);

            try
            {
                response = await _serviceUnitOfWork.NotificationConfigurationService.DeleteBusinessUnitNotificationConfigurationAsync(businessUnitId, processId);
            }
            catch (FaultException<TeleenaInnerExp> exc)
            {
                Logger.LogException(LogSeverity.Error,
                           $"Error while deleting notification configuration for {nameof(businessUnitId)}: {businessUnitId} and {nameof(processId)}: {processId}",
                           nameof(DeleteBusinessUnitNotificationAsync),
                           exc);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(exc.Detail.ErrorCode, nameof(businessUnitId), exc.Detail.ErrorDescription, exc.Detail.TicketId);
            }

            if (!response.Success)
            {
                Logger.Log(LogSeverity.Error,
                           $"Error while deleting notification configuration for {nameof(businessUnitId)}: {businessUnitId} and {nameof(processId)} : {processId}. {response.ErrorMessage}", nameof(DeleteBusinessUnitNotificationAsync));
                return ProviderOperationResult<object>.InvalidInput(nameof(businessUnitId), response.ErrorMessage);
            }

            return ProviderOperationResult<object>.AcceptedResult();
        }
    }
}