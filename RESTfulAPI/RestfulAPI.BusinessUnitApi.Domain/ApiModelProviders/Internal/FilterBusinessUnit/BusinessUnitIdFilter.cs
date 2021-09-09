using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit
{
    public class BusinessUnitIdFilter : BusinessUnitFilterBase
    {
        public override bool CanApplyFilter(GetBusinessUnitRequest request)
        {
            return request != null
                && request.IncludeChildren
                && request.FilterBusinessUnitId.GetValueOrDefault() != Guid.Empty;
        }

        public override List<AccountContract> FilterBusinessUnitsByRequest(List<AccountContract> input, GetBusinessUnitRequest request)
        {
            var root = input.FirstOrDefault(x => x.Id == request.FilterBusinessUnitId);
            if (root == null)
                return new List<AccountContract>();

            var accounts = AddChildrenForRoots(new List<AccountContract>() { root }, input);
            return accounts;
        }
    }
}
