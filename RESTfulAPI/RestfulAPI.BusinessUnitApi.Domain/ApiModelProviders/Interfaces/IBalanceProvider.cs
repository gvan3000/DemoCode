using RestfulAPI.BusinessUnitApi.Domain.Models.BalanceModels;
using RestfulAPI.Common;
using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// Business unit Balance provider
    /// </summary>
    public interface IBalanceProvider
    {
        /// <summary>
        /// Get Business Unit Shared Balances from products
        /// </summary>
        /// <param name="businessUnitId">Business unit Id</param>
        /// <returns>Returns null or populated list of Balances</returns>
        Task<ProviderOperationResult<BalancesResponseModel>> GetBalancesAsync(Guid businessUnitId);

        /// <summary>
        /// Set Shared Balance Amount Per Product
        /// </summary>
        /// <param name="businessUnitId">Id of the business unit</param>
        /// <param name="productId">Id of the product</param>
        /// <param name="request">Contains info about balance per product</param>
        /// <param name="requestId">Id of the request</param>
        /// <returns>Returns 202 HttpStatus Accepted if request is successful</returns>
        Task<ProviderOperationResult<object>> SetBalanceAsync(SetBalanceModel request, Guid businessUnitId, Guid productId, Guid requestId);
    }
}
