using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetPurchasedAddOns;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnCumulativeModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.AddOnService;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    public class AddOnProvider : LoggingBase, IAddOnProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _translators;

        public AddOnProvider(IJsonRestApiLogger logger, ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators translators)
            : base(logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _translators = translators;
        }

        public async Task<ProviderOperationResult<object>> AddAddOnAsync(PurchaseAddOnModel addOn, Guid businessUnitId, Guid requestId)
        {
            if (Equals(addOn, null) || Equals(addOn.AddOnId, null) || Equals(addOn.AddOnId, Guid.Empty))
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(addOn.AddOnId), Constants.MessageConstants.InvalidInputMessage);
            }

            PurchaseGroupAddOnContract input = new PurchaseGroupAddOnContract { AddOnId = addOn.AddOnId, BusinessUnitId = businessUnitId, RequestId = requestId };

            var addOnPurchaseResponse = await _serviceUnitOfWork.AddOnService.PurchaseGroupAddOnAsync(input);

            var result = _translators.PurchaseAddOnResultTranslator.Translate(addOnPurchaseResponse);

            if (result.Fail && !string.IsNullOrEmpty(result.Message))
            {
                return ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.BadRequest, nameof(addOn.AddOnId), result.Message);
            }
            if (result.Fail)
            {
                return ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.InternalServerError, nameof(addOn), Constants.MessageConstants.UnexpectedServiceNullResponseMessage);
            }

            return ProviderOperationResult<object>.AcceptedResult();
        }

        public async Task<ProviderOperationResult<object>> DeleteAddOnAsync(DeleteAddOnModel model, Guid businessUnitId)
        {
            List<BusinessUnitAddOnMatrixxResourceContract> matrixxData;
            try
            {
                matrixxData = await _serviceUnitOfWork.AddOnService.GetBusinessUnitAddOnMatrixxDataByBusinessAsync(businessUnitId, model.AddOnId);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(model), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            if (matrixxData == null || matrixxData.Count == 0)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(DeleteAddOnModel),
                    $"Could not find resource for business unit id: {businessUnitId} and add-on id: {model.AddOnId}");
            }

            var correspondingData = matrixxData.Find(x => x.ResourceId == model.ResourceId);
            bool resourceIdExists = correspondingData != null;

            if (!resourceIdExists)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(model),
                    $"Could not match resource id {model.ResourceId} with supplied business unit id: {businessUnitId} and add-on id: {model.AddOnId}");
            }

            try
            {
                BusinessUnitAddOnCoupleContract coupleContract = await _serviceUnitOfWork.AddOnService.
                    GetBusinessUnitAddOnCoupleByIdAsync(correspondingData.BusinessUnitAddOnCoupleId);

                if (coupleContract.Cancelled.GetValueOrDefault())
                {
                    return ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.Forbidden, nameof(model.AddOnId),
                        $"Add on with id: {model.AddOnId} is already cancelled for the business unit with id: {businessUnitId}");
                }
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(model), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            AddOnCancelParam param = _translators.DeleteAddOnModelTranslator.Translate(model, businessUnitId);
            try
            {
                AddOnSaveResultContract result = await _serviceUnitOfWork.AddOnService.CancelAddOnAsync(param);
                if (!result.Success)
                {
                    string errorMessage = ExtractErrorMessage(result);
                    return ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.InternalServerError, nameof(param), errorMessage.ToString());
                }
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(AddOnCancelParam), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            return ProviderOperationResult<object>.AcceptedResult();
        }

        public async Task<ProviderOperationResult<object>> AddAllowedAddOnsToBusinessUnit(List<Guid> addOnIds, Guid businessUnitId, Guid companyId)
        {
            if (addOnIds == null || addOnIds.Count == 0)
            {
                return ProviderOperationResult<object>.OkResult();
            }

            var serviceContract = _translators.AllowedAddOnsContractTranslator.Translate(addOnIds, businessUnitId);
            try
            {
                await _serviceUnitOfWork.AddOnService.SaveAllowedAddOnsForBusinessUnitAsync(serviceContract);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Could not save allowed add-ons for business unit {businessUnitId}", nameof(AddAllowedAddOnsToBusinessUnit), ex);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(businessUnitId), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }
            return ProviderOperationResult<object>.OkResult();
        }

        public async Task<bool> ValidateAddOns(List<Guid> addOnIds)
        {
            var addOns = await _serviceUnitOfWork.AddOnService.GetAddOnsAsync(addOnIds);

            if (addOns?.Count != addOnIds.Count)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get Addons cumulative response
        /// </summary>
        /// <param name="getPurchasedAddonsRequest">request</param>
        /// <returns></returns>
        public async Task<AddOnCumulativeListModel> GetAddOnsAsync(GetPurchasedAddonsBusinessUnitRequest getPurchasedAddonsRequest)
        {
            var request = new GetPurchasedAddonsBusinessUnitContract
            {
                BusinessUnitId = getPurchasedAddonsRequest.BusinessUnitId,
                IncludeExpired = getPurchasedAddonsRequest.IncludeExpired
            };

            var addOnServiceResponse = await _serviceUnitOfWork.AddOnService.GetBusinessUnitPurchasedAddOnContractAsync(request);

            if (addOnServiceResponse == null || addOnServiceResponse.BusinessUnitPurchasedAddOns == null)
            {
                return null;
            }

            var result = _translators.BusinessUnitPurchasedAddOnsCumulativeTranslator.Translate(addOnServiceResponse);
            return result;
        }

        private string ExtractErrorMessage(AddOnSaveResultContract contract)
        {
            StringBuilder errorMessage = new StringBuilder();

            for (int i = 0; contract.Errors != null && contract.Errors.Count > i; i++)
            {
                errorMessage.AppendLine(contract.Errors[i]);
            }

            return errorMessage.ToString();
        }
    }
}
