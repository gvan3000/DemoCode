using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableSIMs;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableSIMModels;
using RestfulAPI.Common;
using System;
using System.Threading.Tasks;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    public interface ISimProvider
    {
        Task<ProviderOperationResult<AvailableSIMResponseModel>> GetAvailableSIMsAsync(Guid businessUnitId, string status);

        Task<ProviderOperationResult<AvailableSimResponseV2Model>> GetAvailableSIMsAsync(AvailableSimProviderRequest request);
    }
}
