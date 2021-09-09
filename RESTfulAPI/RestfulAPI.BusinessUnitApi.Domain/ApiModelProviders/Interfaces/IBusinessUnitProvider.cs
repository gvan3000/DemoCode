using System;
using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// Business unit model provider for REST api
    /// </summary>
    public interface IBusinessUnitProvider
    {
        /// <summary>
        /// Create a single business unit
        /// </summary>
        /// <param name="userCompanyId"></param>
        /// <param name="userBusinessUnitId"></param>
        /// <param name="request">input contract received from WebApi</param>
        /// <param name="requestId">Request id from headers</param>
        /// <returns>data contract with id of the newly craeted busines unit</returns>
        Task<ProviderOperationResult<CreateBusinessUnitResponseModel>> CreateAsync(Guid userCompanyId, Guid? userBusinessUnitId, BusinessUnitCreateModel request, Guid requestId);

        /// <summary>
        /// Get the business unit details
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<BusinessUnitListModel> GetBusinessUnitsWithFilteringAsync(GetBusinessUnitRequest request);

        /// <summary>
        /// Updates single business unit with full model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<object>> UpdateBusinessUnitAsync(Guid id, BusinessUnitPatchModel model);
    }
}
