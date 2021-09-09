using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.APNs
{
    /// <summary>
    /// Delete apn validator
    /// </summary>
    public interface IDeleteApnValidator
    {
        /// <summary>
        /// Validate provided apn name against apns linked to the business unit
        /// </summary>
        /// <param name="apns">apns linked to the business unit</param>
        /// <param name="removeApn">id of the apn to remove</param>
        /// <returns></returns>
        ProviderOperationResult<object> Validate(List<ApnDetailContract> apns, Guid removeApn);
    }
}
