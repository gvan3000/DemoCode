using System;
using System.Security.Claims;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.QuotaModels;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    public interface IQuotaDistributionProvider
    {
        /// <summary>
        /// Retrieves shared quota limit for product
        /// </summary>
        /// <param name="accountId">id of business unit (account)</param>
        /// <param name="productId">id of product</param>
        /// <returns>object with result of error message if error occured</returns>
        Task<ProviderOperationResult<BalanceQuotasListModel>> GetSharedBalancesForProductAsync(Guid accountId, Guid productId);

        /// <summary>
        /// Retrieves all shared quotas for products on business unit
        /// </summary>
        /// <param name="businessUnitId">business unit id (account)</param>
        /// <returns>object with result or error message if error has occured</returns>
        Task<ProviderOperationResult<ProductAllowedBalanceList>> GetAllSharedBalancesPerBusinessUnitAsync(Guid businessUnitId);

        /// <summary>
        /// Sets new quota limit for business unit (shared balance)
        /// </summary>
        /// <param name="accountId">id of business unit (account)</param>
        /// <param name="input">input data</param>
        /// <param name="user">authenticated user making the call</param>
        /// <returns>object indicating if operation was successful or not</returns>
        Task<ProviderOperationResult<object>> SetBusinessUnitQuota(Guid accountId, SetBusinessUnitQuotaModel input, ClaimsPrincipal user);
    }
}