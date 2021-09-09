using System.Linq;
using System.Collections.Generic;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit
{
    public class BusinessUnitHasSharedWalletFilter : BusinessUnitFilterBase
    {
        public override bool CanApplyFilter(GetBusinessUnitRequest request)
        {
            return request != null && !string.IsNullOrEmpty(request.FilterHasSharedWallet);
        }

        public override List<AccountContract> FilterBusinessUnitsByRequest(List<AccountContract> input, GetBusinessUnitRequest request)
        {
            List<AccountContract> result;

            bool isSharedWallet;

            if (bool.TryParse(request.FilterHasSharedWallet, out isSharedWallet))
            {
                result = input.Where(bu => bu.IsSharedWallet == isSharedWallet).ToList();
            }
            else
            {
                result = new List<AccountContract>();
            }

            return result;
        }
    }
}
