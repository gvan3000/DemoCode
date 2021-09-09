using System.Threading.Tasks;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn;
using RestfulAPI.BusinessUnitApi.Domain.Models.AvailableMSISDNModels;
using RestfulAPI.Common;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Interfaces
{
    public interface IMobileProvider
    {
        /// <summary>
        /// Gets Available MSISDNS per Business Unit
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<ProviderOperationResult<AvailableMSISDNResponseModel>> GetAvailableMsisdnsAsync(AvailableMsisdnProviderRequest request);
    }
}
