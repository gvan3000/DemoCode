using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetPurchasedAddOns;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnCumulativeModels;
using RestfulAPI.BusinessUnitApi.Domain.Models.AddOnModels;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// Business Unit AddOn Provider
    /// </summary>
    public interface IAddOnProvider
    {
        /// <summary>
        /// Adds a shared add-on to the business unit
        /// </summary>
        /// <param name="addOn"></param>
        /// <param name="businessUnitId"></param>
        /// <param name="requestId">Request id from header</param>
        /// <returns>Returns success result and error message</returns>
        Task<ProviderOperationResult<object>> AddAddOnAsync(PurchaseAddOnModel addOn, Guid businessUnitId, Guid requestId);

        /// <summary>
        /// Add Allowed AddOns To BusinessUnit
        /// </summary>
        /// <param name="addOnIds"></param>
        /// <param name="businessUnitId"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<object>> AddAllowedAddOnsToBusinessUnit(List<Guid> addOnIds, Guid businessUnitId, Guid companyId);

        /// <summary>
        /// Check if all addOn Ids exists
        /// </summary>
        /// <param name="addOnIds"></param>
        /// <returns></returns>
        Task<bool> ValidateAddOns(List<Guid> addOnIds);

        /// <summary>
        /// Get add-ons added to the business unit; cumulative response TODO: update description
        /// </summary>
        /// <param name="getPurchasedAddonsRequest"></param>
        /// <returns></returns>
        Task<AddOnCumulativeListModel> GetAddOnsAsync(GetPurchasedAddonsBusinessUnitRequest getPurchasedAddonsRequest);

        /// <summary>
        /// Removes the add-on from the business unit.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<object>> DeleteAddOnAsync(DeleteAddOnModel model, Guid businessUnitId);
    }
}
