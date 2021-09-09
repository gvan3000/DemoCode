using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.APNs
{
    public class UpdateBusinessUnitApnsValidator : IUpdateBusinessUnitApnsValidator
    {
        public ProviderOperationResult<object> ValidateModel(UpdateBusinessUnitApnsModel input, ApnSetWithDetailsContract[] availableApns)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            if ((input.Apns == null || !input.Apns.Any()) && input.DefaultApn != null)
                return ProviderOperationResult<object>.InvalidInput(nameof(input.DefaultApn), $"Default APN can't be set when list of APNs is empty");

            if (input.DefaultApn == null && input.Apns.Count != 0)
                return ProviderOperationResult<object>.InvalidInput(nameof(input.DefaultApn), $"Default APN must not be empty");

            if (input.DefaultApn != null && !input.Apns.Select(x => x.Id).Contains(input.DefaultApn.Value))
                return ProviderOperationResult<object>.InvalidInput(nameof(input.DefaultApn), $"Default APN must be in list of available APNs");

            if (input.Apns != null)
            {
                if (input.Apns.Count != input.Apns.Select(x => x.Id).Distinct().Count())
                    return ProviderOperationResult<object>.InvalidInput(nameof(input.Apns), $"Duplicate APNs are not allowed");

                if ((availableApns == null || !availableApns.Any()) && input.Apns.Any())
                    return ProviderOperationResult<object>.InvalidInput(nameof(input.Apns), $"No company level APNs defined, setting business unit level APNs is not possible.");

                var available = availableApns.SelectMany(x => x.ApnSetDetails.Select(detail => detail.ApnSetDetailId)).Distinct();
                if (available.Intersect(input.Apns.Select(x => x.Id)).Count() != input.Apns.Count)
                    return ProviderOperationResult<object>.InvalidInput(nameof(input.Apns), $"APN list contains entries not defined for company available APNs");
            }

            return ProviderOperationResult<object>.OkResult();
        }
    }
}
