using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ApnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.APNs
{
    public interface IUpdateBusinessUnitDefaultApnValidator
    {
        ProviderOperationResult<object> ValidateModel(UpdateBusinessUnitDefaultApnModel input, ApnDetailContract[] businessUnitApns);
    }
}