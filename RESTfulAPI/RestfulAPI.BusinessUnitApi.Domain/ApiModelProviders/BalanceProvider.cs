using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.TeleenaServiceReferences;
using System;
using System.Threading.Tasks;
using RestfulAPI.Common;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using System.Net;
using System.ServiceModel;
using RestfulAPI.Logging;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.Balance;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using RestfulAPI.Constants;
using TeleenaLogging.Abstraction;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    /// <summary>
    /// Balance Provider
    /// </summary>
    public class BalanceProvider : LoggingBase, IBalanceProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _translators;
        private const string SHARED_PROP_TYPECODE = "SHB";

        /// <summary>
        /// Initialize Balance Provider
        /// </summary>
        /// <param name="serviceUnitOfWork"></param>
        /// <param name="translators"></param>
        /// <param name="logger"></param>
        public BalanceProvider(ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators translators, IJsonRestApiLogger logger)
            : base(logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _translators = translators;
        }

        /// <summary>
        /// Get a list of all balances of the business unit, if available
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<BalancesResponseModel>> GetBalancesAsync(Guid businessUnitId)
        {
            if (businessUnitId == Guid.Empty)
            {
                throw new ArgumentException(nameof(businessUnitId));
            }

            AccountBalanceRequest input = new AccountBalanceRequest
            {
                AccountId = businessUnitId,
                ReturnRawValues = false //Calculate the balance amount divided by ConversionRate
            };

            AccountBalanceWithBucketsContract[] accountBalances;
            try
            {
                accountBalances = await _serviceUnitOfWork.BalanceService.GetAccountBalancesAsync(input);
            }
            catch (FaultException<TeleenaServiceReferences.BalanceService.TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Getting balances for account id {input.AccountId} failed.", nameof(GetBalancesAsync), ex);
                return ProviderOperationResult<BalancesResponseModel>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(input), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            BalancesResponseModel result = _translators.BalanceTranslator.Translate(accountBalances);

            if (result?.Balances == null || result.Balances.Count == 0)
            {
                return ProviderOperationResult<BalancesResponseModel>.NotFoundResult($"There are no balances for requested business unit with id of {businessUnitId}");
            }

            return ProviderOperationResult<BalancesResponseModel>.OkResult(result);
        }

        /// <summary>
        /// Set the amount to be used by a product on a shared balance
        /// </summary>
        /// <param name="request"></param>
        /// <param name="businessUnitId">Id of the business unit</param>
        /// <param name="productId">Id of the product</param>
        /// <param name="requestId">RequestId header value</param>
        /// <returns></returns>
        public async Task<ProviderOperationResult<object>> SetBalanceAsync(SetBalanceModel request, Guid businessUnitId, Guid productId, Guid requestId)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var product = await _serviceUnitOfWork.ProductService.GetProductByIdAsync(productId);

            if (product?.AccountId != businessUnitId)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(productId), $"Product : {productId} does not belong to the BusinessUnit : {businessUnitId}");
            }

            SaveQuotaDistributionResultContract serviceResponse = null;

            var propositions = await _serviceUnitOfWork.PropositionService.GetActivePropositionsByBusinessUnitAsync(businessUnitId);

            var propositionsInfo = _translators.PropositionInfoModelTranslator.Translate(propositions);

            var shbSubscriptionTypePropositions = FilterPropositionsByShbSubscriptionType(propositionsInfo);

            if (shbSubscriptionTypePropositions == null || shbSubscriptionTypePropositions.Count == 0)
            {
                return ProviderOperationResult<object>.InvalidInput($"{nameof(businessUnitId)}", $"Busines Unit - {businessUnitId} is not a Shared Business Unit");
            }

            var shbEusProposition = await GetEndUserSubscriptionProposition(shbSubscriptionTypePropositions, businessUnitId);

            if (shbEusProposition == null)
            {
                return ProviderOperationResult<object>.InvalidInput($"{nameof(businessUnitId)}", $"Busines Unit - {businessUnitId} is not a Shared Business Unit");
            }

            bool validServiceTypeCode = ValidateProvidedServiceTypeCode(request, shbEusProposition);

            if (!validServiceTypeCode)
            {
                return ProviderOperationResult<object>.InvalidInput($"{nameof(request.ServiceTypeCode)}", $"Wrong {nameof(request.ServiceTypeCode)} value provided");
            }

            if (request.ServiceTypeCode.ToString().Equals(BalanceConstants.SERVICE_TYPE_CODE_QUOTA_NAME, StringComparison.InvariantCultureIgnoreCase))
            {
                var copContract = await _serviceUnitOfWork.CommercialOfferService.GetCommercailOfferPropositionForAsync(shbEusProposition.CommercialOfferPropositionCode);
                shbEusProposition.QuotaOfferCode = copContract.QuotaOfferCode;
            }
 
            var saveQuoataDistributionContract = _translators.SaveQuotaDistributionContactTranslator.Translate(businessUnitId, productId, shbEusProposition, request);
            saveQuoataDistributionContract.RequestId = requestId;

            try
            {
                serviceResponse = await _serviceUnitOfWork.QuotaDistributionService.SaveQuotaDistributionAsync(saveQuoataDistributionContract);
            }
            catch (FaultException<TeleenaServiceReferences.QuotaDistributionService.TeleenaInnerExp> exc)
            {
                Logger.LogException(LogSeverity.Error, "Save Quota Distribution error occured", nameof(SetBalanceAsync), exc.InnerException);
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(exc.Detail.ErrorCode, nameof(request), exc.Detail.ErrorDescription, exc.Detail.TicketId);
            }

            if (serviceResponse != null && serviceResponse.Success)
            {
                return ProviderOperationResult<object>.AcceptedResult();
            }
            else
            {
                var errorMessages = GetErrorMessages(serviceResponse);

                return ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.InternalServerError, $"{nameof(request)}", errorMessages);
            }            
        }

        private List<PropositionInfoModel> FilterPropositionsByShbSubscriptionType(List<PropositionInfoModel> allPropositionsInfo)
        {
            List<PropositionInfoModel> shbPropositions = new List<PropositionInfoModel>();

            if (allPropositionsInfo == null)
            {
                return shbPropositions;
            }

            foreach (var propositionInfo in allPropositionsInfo)
            {
                var shbCommercialOffers = propositionInfo.CommercialOfferDefinitions?.Where(c => c.SubscriptionTypeCode == SHARED_PROP_TYPECODE).ToList();

                if (shbCommercialOffers.Any())
                {
                    shbPropositions.Add(new PropositionInfoModel
                    {
                        PropositionId = propositionInfo.PropositionId,
                        CommercialOfferDefinitions = shbCommercialOffers,
                         CommercialOfferPropositionCode = propositionInfo.CommercialOfferPropositionCode
                    });
                }
            }

            return shbPropositions;
        }

        private bool ValidateProvidedServiceTypeCode(SetBalanceModel request, PropositionInfoModel shbEusProposition)
        {
            bool isValid = false;

            if (shbEusProposition.CommercialOfferDefinitions.Any(c => c.ServiceTypeCode == request.ServiceTypeCode.ToString().ToUpperInvariant()))
            {
                isValid = true;
            }

            if (request.ServiceTypeCode.ToString().Equals(BalanceConstants.SERVICE_TYPE_CODE_QUOTA_NAME, StringComparison.InvariantCultureIgnoreCase))
            {
                isValid = true;
            }

            return isValid;
        }

        private string GetErrorMessages(SaveQuotaDistributionResultContract serviceResponse)
        {
            if (serviceResponse?.Errors == null)
            {
                return string.Empty;
            }

            StringBuilder errorMessages = new StringBuilder();

            foreach (var error in serviceResponse.Errors)
            {
                errorMessages.AppendLine(error);
            }

            return errorMessages.ToString();
        }

        private async Task<PropositionInfoModel> GetEndUserSubscriptionProposition(List<PropositionInfoModel> shbSubscriptionTypePropositions, Guid businessUnitId) 
        {
            PropositionInfoModel shbEusProposition = null;
            foreach (var proposition in shbSubscriptionTypePropositions)
            {
                var eusPropositionInfo = await _serviceUnitOfWork.PropositionService.GetEndUserSubscriptionInfoAsync(businessUnitId, proposition.PropositionId);

                if (eusPropositionInfo != null && eusPropositionInfo.EndUserSubscription)
                {
                    proposition.EndUserSubscription = eusPropositionInfo.EndUserSubscription;
                    shbEusProposition = proposition;
                    break;
                }
            }

            return shbEusProposition;
        }
                
    }
}
