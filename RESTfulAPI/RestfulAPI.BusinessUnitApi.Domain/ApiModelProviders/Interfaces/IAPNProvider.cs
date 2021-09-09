using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.Common;
using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    /// <summary>
    /// Manupulates busienss unit apns and accesses company level ones
    /// </summary>
    public interface IAPNProvider
    {
        Task<APNSetList> GetCompanyAPNsAsync(Guid userCompanyId);

        /// <summary>
        /// Updates list of apns for business unit
        /// </summary>
        /// <param name="businessUnitId">business unit id</param>
        /// <param name="companyId">company id from user's claim</param>
        /// <param name="input">list of apns with default one</param>
        /// <returns>indication whether operation was successful or not</returns>
        Task<ProviderOperationResult<object>> UpdateBusinessUnitApnsAsync(Guid businessUnitId, Guid companyId, UpdateBusinessUnitApnsModel input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<APNsResponseModel>> GetAPNsAsync(Guid businessUnitId);


        /// <summary>
        /// Update Default Apn of BusinessUnit
        /// </summary>
        /// <param name="businessUnitId"></param>
        /// <param name="companyId"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<object>> UpdateBusinessUnitDefaultApnAsync(Guid businessUnitId, Guid companyId, UpdateBusinessUnitDefaultApnModel input);

        /// <summary>
        /// Removes the link between the APN and the business unit
        /// </summary>
        /// <param name="businessUnitId">Id of the busines unit</param>
        /// <param name="apnId">Id of the APN</param>
        /// <returns></returns>
        Task<ProviderOperationResult<object>> RemoveApnAsync(Guid businessUnitId, Guid apnId);
    }
}
