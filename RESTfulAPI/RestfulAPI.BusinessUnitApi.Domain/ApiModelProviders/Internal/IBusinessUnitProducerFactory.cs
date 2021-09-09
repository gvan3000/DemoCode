using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal
{
    public interface IBusinessUnitProducerFactory
    {
        IBusinessUnitLoadingStrategy GetLoader(GetBusinessUnitRequest request);
        IBusinessUnitFilter GetFilterForRequest(GetBusinessUnitRequest request);
    }
}
