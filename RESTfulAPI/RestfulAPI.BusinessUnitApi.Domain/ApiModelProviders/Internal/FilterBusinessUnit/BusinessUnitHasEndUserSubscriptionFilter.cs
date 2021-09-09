using System.Collections.Generic;
using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit
{
    /// <summary>
    /// Filter by HasEndUserSubscription query
    /// </summary>
    public class BusinessUnitHasEndUserSubscriptionFilter : IBusinessUnitFilter
    {
        /// <summary>
        /// return capabilities
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool CanApplyFilter(GetBusinessUnitRequest request)
        {
            return request != null
                && request.FilterHasEndUserSubscripion != null
                && request.FilterHasSharedWallet == null
                && request.FilterCustomerId == null
                && request.FilterBusinessUnitName == null
                && request.IncludeChildren == false
                && request.FilterBusinessUnitId == null;
        }

        /// <summary>
        /// Perform filtering
        /// </summary>
        /// <param name="input"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public List<AccountContract> FilterBusinessUnitsByRequest(List<AccountContract> input, GetBusinessUnitRequest request)
        {
            var filtered = input
                .Where(x => x.HasEndUserSubscription == request.FilterHasEndUserSubscripion.GetValueOrDefault())
                .ToList();
            return filtered;
        }
    }
}
