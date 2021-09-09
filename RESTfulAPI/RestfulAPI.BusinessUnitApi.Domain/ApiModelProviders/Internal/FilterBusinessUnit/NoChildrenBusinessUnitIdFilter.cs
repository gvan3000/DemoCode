using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit
{
    public class NoChildrenBusinessUnitIdFilter : IBusinessUnitFilter
    {
        public bool CanApplyFilter(GetBusinessUnitRequest request)
        {
            return request != null
                && !request.IncludeChildren
                && request.FilterBusinessUnitId.GetValueOrDefault() != Guid.Empty;
        }

        public List<AccountContract> FilterBusinessUnitsByRequest(List<AccountContract> input, GetBusinessUnitRequest request)
        {
            return input.Where(x => x.Id == request.FilterBusinessUnitId).ToList();
        }
    }
}
