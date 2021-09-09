using System;
using System.Collections.Generic;
using System.Linq;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System.Net;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.APNs
{
    /// <summary>
    /// Delete apn validator
    /// </summary>
    public class DeleteApnValidator : IDeleteApnValidator
    {
        /// <summary>
        /// Validate provided apn name to remove against business unit apns
        /// </summary>
        /// <param name="apns">business unit apns</param>
        /// <param name="removeApnSetDetailId">provided apn id to remove</param>
        /// <returns>Validation result</returns>
        public ProviderOperationResult<object> Validate(List<ApnDetailContract> apns, Guid removeApnSetDetailId)
        {
            if (apns == null)
            {
                throw new ArgumentNullException(nameof(apns));
            }

            if (!apns.Any())
            {
                return ProviderOperationResult<object>.GenerateFailureResult(HttpStatusCode.InternalServerError, nameof(apns), "Could not find apns");
            }           

            var apnToRemoveExists = apns.Any(x => x.ApnSetDetailId == removeApnSetDetailId);

            if (!apnToRemoveExists)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(removeApnSetDetailId), $"Could not find apn with id: {removeApnSetDetailId}");
            }

            var defaultApn = apns.FirstOrDefault(x => x.IsDefault);

            if (defaultApn.ApnSetDetailId == removeApnSetDetailId)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(removeApnSetDetailId), $"Default apn: {removeApnSetDetailId} cannot be removed");
            }

            return ProviderOperationResult<object>.OkResult();
        }
    }
}
