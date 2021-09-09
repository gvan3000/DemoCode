using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ApnService;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.APNs
{
    public class UpdateBusinessUnitDefaultApnValidator : IUpdateBusinessUnitDefaultApnValidator
    {
        public ProviderOperationResult<object> ValidateModel(UpdateBusinessUnitDefaultApnModel input, ApnDetailContract[] businessUnitApns)
        {
            if (businessUnitApns == null || businessUnitApns.Length == 0)
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(input), $"No business unit APNs found for selected business unit, default APN can't be set");
            }
            if (!businessUnitApns.Any(x => x.ApnSetDetailId == input.Id))
            {
                return ProviderOperationResult<object>.InvalidInput(nameof(input.Id), $"Invalid default APN id supplied. The default APN must be in list of business unit APNs");
            }

            return ProviderOperationResult<object>.OkResult();
        }
    }
}
