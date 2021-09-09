using RestfulAPI.BusinessUnitApi.Domain.Models.APNs;
using RestfulAPI.Common;
using RestfulAPI.TeleenaServiceReferences.ApnService;

namespace RestfulAPI.BusinessUnitApi.Domain.Validators.APNs
{
    public interface IUpdateBusinessUnitApnsValidator
    {
        ProviderOperationResult<object> ValidateModel(UpdateBusinessUnitApnsModel input, ApnSetWithDetailsContract[] availableApns);
    }
}
