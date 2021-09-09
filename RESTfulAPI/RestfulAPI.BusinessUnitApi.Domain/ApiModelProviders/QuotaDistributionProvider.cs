using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.ServiceModel;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.QuotaModels;
using RestfulAPI.BusinessUnitApi.Domain.Translators;
using RestfulAPI.Common;
using RestfulAPI.Constants;
using RestfulAPI.Logging;
using RestfulAPI.TeleenaServiceReferences;
using RestfulAPI.TeleenaServiceReferences.BalanceService;
using RestfulAPI.TeleenaServiceReferences.QuotaDistributionService;
using TeleenaLogging.Abstraction;
using TeleenaInnerExp = RestfulAPI.TeleenaServiceReferences.BalanceService.TeleenaInnerExp;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders
{
    public class QuotaDistributionProvider : LoggingBase, IQuotaDistributionProvider
    {
        private readonly ITeleenaServiceUnitOfWork _serviceUnitOfWork;
        private readonly IBusinessUnitApiTranslators _translators;

        /// <summary>
        /// QuotaDistributionProvider initialize
        /// </summary>
        /// <param name="serviceUnitOfWork"></param>
        /// <param name="translators"></param>
        /// <param name="logger"></param>
        public QuotaDistributionProvider(ITeleenaServiceUnitOfWork serviceUnitOfWork, IBusinessUnitApiTranslators translators, IJsonRestApiLogger logger)
            : base(logger)
        {
            _serviceUnitOfWork = serviceUnitOfWork;
            _translators = translators;
        }

        /// <summary>
        /// Get all products with the maximum allowed usage per balance
        /// </summary>
        /// <param name="businessUnitId">Id of the business unit</param>
        /// <returns>List of all products with maximum allowed usage per balance</returns>
        public async Task<ProviderOperationResult<ProductAllowedBalanceList>> GetAllSharedBalancesPerBusinessUnitAsync(Guid businessUnitId)
        {
            var businessUnit = await _serviceUnitOfWork.AccountService.GetAccountByIdAsync(businessUnitId);
            if (!businessUnit.IsSharedWallet)
            {
                return ProviderOperationResult<ProductAllowedBalanceList>.InvalidInput("BusinessUnit", $"Business unit does not have a shared balance.");
            }

            ProductAllowedBalancesContract[] sharedBalancesData = null;
            try
            {
                sharedBalancesData = await _serviceUnitOfWork.QuotaDistributionService.GetSharedBalanceForAccountAsync(businessUnitId);
            }
            catch (FaultException<TeleenaServiceReferences.QuotaDistributionService.TeleenaInnerExp> exc)
            {
                Logger.LogException(LogSeverity.Error, $"Error getting shared balances for the account: {businessUnitId}", nameof(GetAllSharedBalancesPerBusinessUnitAsync), exc);
                return ProviderOperationResult<ProductAllowedBalanceList>.TeleenaExceptionAsResult(exc.Detail.ErrorCode, nameof(businessUnitId), exc.Detail.ErrorDescription, exc.Detail.TicketId);
            }

            if (sharedBalancesData == null || sharedBalancesData.Length < 1)
            {
                return ProviderOperationResult<ProductAllowedBalanceList>.NotFoundResult($"Allowed balances does not exists for the business unit id: {businessUnitId} ");
            }

            var translatedResult = _translators.ProductAllowedBalancesTranslator.Translate(sharedBalancesData.ToList());

            return ProviderOperationResult<ProductAllowedBalanceList>.OkResult(translatedResult);
        }

        public async Task<ProviderOperationResult<BalanceQuotasListModel>> GetSharedBalancesForProductAsync(Guid accountId, Guid productId)
        {
            var product = await _serviceUnitOfWork.ProductService.GetProductByIdAsync(productId);
            if (product.AccountId != accountId)
            {
                return ProviderOperationResult<BalanceQuotasListModel>
                    .InvalidInput(nameof(productId), $"Product does not belong to account - {accountId}.");
            }

            var businessUnit = await _serviceUnitOfWork.AccountService.GetAccountByIdAsync(accountId);
            if (!businessUnit.IsSharedWallet)
            {
                return ProviderOperationResult<BalanceQuotasListModel>
                    .InvalidInput("BusinessUnit", $"Business unit does not have a shared balance.");
            }

            //Get default balances with initial amounts
            var defaultBalancesProviderResult = await GetDefaultBalancesAsync(accountId);
            var defaultBalances = defaultBalancesProviderResult.IsSuccess ?
                                  defaultBalancesProviderResult.Result :
                                  new BalanceQuotasListModel() { BalanceAllowances = new List<BalanceQuotaModel>() };

            //Get Shared Balance For Product
            var serviceResponse = await _serviceUnitOfWork.QuotaDistributionService.GetSharedBalanceForProductAsync(productId);
            if (serviceResponse == null)
            {
                Logger.Log(LogSeverity.Info, $"Balance quotas does not exist for product with id {productId}", nameof(GetSharedBalancesForProductAsync));
            }
            var translatedResultOfOverridenBalances = _translators.ProductQuotaDistributionContractTranslator.Translate(serviceResponse);
            var overrridenBalances = translatedResultOfOverridenBalances?.BalanceAllowances != null ?
                                     translatedResultOfOverridenBalances :
                                     new BalanceQuotasListModel() { BalanceAllowances = new List<BalanceQuotaModel>() };

            if (!defaultBalances.BalanceAllowances.Any() && !overrridenBalances.BalanceAllowances.Any())
            {
                return ProviderOperationResult<BalanceQuotasListModel>.NotFoundResult($"No shared balances for product with id {productId}");
            }

            var finalResult = ComposeFinalBalances(defaultBalances, overrridenBalances);

            return ProviderOperationResult<BalanceQuotasListModel>.OkResult(finalResult);
        }


        private async Task<ProviderOperationResult<BalanceQuotasListModel>> GetDefaultBalancesAsync(Guid businessUnitId)
        {
            if (businessUnitId == Guid.Empty)
            {
                throw new ArgumentException(nameof(businessUnitId));
            }

            AccountBalanceRequest input = new AccountBalanceRequest
            {
                AccountId = businessUnitId,
                ReturnRawValues = false
            };

            AccountBalanceWithBucketsContract[] accountBalances;
            try
            {
                accountBalances = await _serviceUnitOfWork.BalanceService.GetAccountBalancesAsync(input);
            }
            catch (FaultException<TeleenaServiceReferences.BalanceService.TeleenaInnerExp> ex)
            {
                Logger.LogException(LogSeverity.Error, $"Getting account balances for account id {input.AccountId} failed.", nameof(GetDefaultBalancesAsync), ex);
                return ProviderOperationResult<BalanceQuotasListModel>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, nameof(input), ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            var result = _translators.BalancesContractBalanceQuotaListModelTranslator.Translate(accountBalances);

            if (result == null || result.BalanceAllowances == null || result.BalanceAllowances.Count == 0)
            {
                return ProviderOperationResult<BalanceQuotasListModel>.NotFoundResult($"There are no balances for requested business unit with id of {businessUnitId}");
            }

            return ProviderOperationResult<BalanceQuotasListModel>.OkResult(result);
        }

        private BalanceQuotasListModel ComposeFinalBalances(BalanceQuotasListModel defaultBalances, BalanceQuotasListModel overridenBalances)
        {
            var finalBalances = new BalanceQuotasListModel() { BalanceAllowances = new List<BalanceQuotaModel>() };

            foreach (var defaultItem in defaultBalances.BalanceAllowances)
            {
                var overridenBalance = overridenBalances.BalanceAllowances.Find(x => defaultItem.ServiceTypeCode == x.ServiceTypeCode);
                if (overridenBalance != null)
                {
                    overridenBalance.IsCapped = true;
                    finalBalances.BalanceAllowances.Add(overridenBalance);
                    overridenBalances.BalanceAllowances.Remove(overridenBalance);
                }
                else
                {
                    finalBalances.BalanceAllowances.Add(defaultItem);
                }
            }

            finalBalances.BalanceAllowances.AddRange(overridenBalances.BalanceAllowances);// Add remaining overrides if any

            return finalBalances;
        }

        public async Task<ProviderOperationResult<object>> SetBusinessUnitQuota(Guid accountId, SetBusinessUnitQuotaModel input, ClaimsPrincipal user)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException($"Must be a valid id from database", nameof(accountId));
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (user == null)
                throw new ArgumentNullException(nameof(input));

            var account = await _serviceUnitOfWork.AccountService.GetAccountByIdAsync(accountId);
            if (account == null)
                return ProviderOperationResult<object>.GenerateFailureResult(System.Net.HttpStatusCode.NotFound, "id", "business unit does not exist");

            var allowedPropositions = await _serviceUnitOfWork.PropositionService.GetActivePropositionsByBusinessUnitForProductCreationAsync(accountId);
            if (allowedPropositions == null
                || allowedPropositions.PropositionContracts == null
                || !allowedPropositions.PropositionContracts.Any(x => x.CommercialOfferConfigurationsContract.CommercialOfferConfigurationContracts.Any(co => co.IsSharedWallet)))
            {
                return ProviderOperationResult<object>.InvalidInput("id", "Business unit does not have shared balances");
            }

            var companyBalanceRequest = new GetCompanyBalanceTypeTopUpSettingByCompanyContract()
            {
                CompanyId = account.CompanyId.GetValueOrDefault()
            };
            var allBalanceTypesForAccount = await _serviceUnitOfWork.BalanceService.GetCompanyBalanceTypeTopUpSettingByCompanyAsync(companyBalanceRequest);
            if (allBalanceTypesForAccount == null || !allBalanceTypesForAccount.Any())
                return ProviderOperationResult<object>.GenerateFailureResult(System.Net.HttpStatusCode.InternalServerError, "", "Could not find balance types for company");
            var generalCash = allBalanceTypesForAccount.FirstOrDefault(x => x.BalanceTypeName.Equals(BalanceConstants.SHARED_BALANCE_TOPUP_NAME, StringComparison.InvariantCultureIgnoreCase));
            if (generalCash == null)
                return ProviderOperationResult<object>.GenerateFailureResult(System.Net.HttpStatusCode.BadRequest, "", "General cash quota could not be set, its not configured for company");

            var serviceRequest = _translators.SetQuotaTranslator.Translate(accountId, input, user, generalCash);

            try
            {
                await _serviceUnitOfWork.BalanceService.CustomTopUpOnAccountAsync(serviceRequest);
            }
            catch (FaultException<TeleenaInnerExp> ex)
            {
                return ProviderOperationResult<object>.TeleenaExceptionAsResult(ex.Detail.ErrorCode, "request", ex.Detail.ErrorDescription, ex.Detail.TicketId);
            }

            return ProviderOperationResult<object>.AcceptedResult();
        }
    }
}
