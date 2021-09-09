using System;
using System.Collections.Generic;
using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit
{
    public class NoChildrenNameFilter : IBusinessUnitFilter
    {
        public bool CanApplyFilter(GetBusinessUnitRequest request)
        {
            return request != null
                //&& !request.IncludeChildren
                && !string.IsNullOrEmpty(request.FilterBusinessUnitName);
        }

        public List<AccountContract> FilterBusinessUnitsByRequest(List<AccountContract> input, GetBusinessUnitRequest request)
        {
            return input.Where(x => x.UserName.Equals(request.FilterBusinessUnitName, StringComparison.InvariantCultureIgnoreCase)).ToList();
        }
    }
}
