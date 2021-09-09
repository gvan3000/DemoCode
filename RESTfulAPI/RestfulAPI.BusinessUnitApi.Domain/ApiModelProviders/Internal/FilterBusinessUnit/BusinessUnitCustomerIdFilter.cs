using System.Collections.Generic;
using System.Linq;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit
{
    public class BusinessUnitCustomerIdFilter : BusinessUnitFilterBase
    {
        public override bool CanApplyFilter(GetBusinessUnitRequest request)
        {
            return request != null
                && !string.IsNullOrEmpty(request.FilterCustomerId);
        }

        public override List<AccountContract> FilterBusinessUnitsByRequest(List<AccountContract> input, GetBusinessUnitRequest request)
        {
            var result = input.Where(bu => bu.CustomerNumber == request.FilterCustomerId).ToList();

            if (request.IncludeChildren)
                result = AddChildrenForRoots(result, input);

            return result;
        }
    }
}
