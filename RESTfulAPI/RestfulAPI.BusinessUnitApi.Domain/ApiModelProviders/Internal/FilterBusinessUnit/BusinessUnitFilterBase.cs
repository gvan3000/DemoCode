using RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts;
using RestfulAPI.TeleenaServiceReferences.AccountService;
using System.Collections.Generic;
using System.Linq;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.FilterBusinessUnit
{
    public abstract class BusinessUnitFilterBase: IBusinessUnitFilter
    {
        public abstract bool CanApplyFilter(GetBusinessUnitRequest request);
        public abstract List<AccountContract> FilterBusinessUnitsByRequest(List<AccountContract> input, GetBusinessUnitRequest request);

        protected List<AccountContract> AddChildrenForRoots(List<AccountContract> roots, List<AccountContract> input)
        {
            var accounts = new List<AccountContract>();
            accounts.AddRange(roots);
            
            var lastBatchSize = accounts.Count;
            while (lastBatchSize > 0)
            {
                var compareRange = accounts.GetRange(accounts.Count - lastBatchSize, lastBatchSize);
                var insert = input.Where(x => compareRange.Any(cr => cr.Id == x.ParentId));
                accounts.AddRange(insert);
                lastBatchSize = insert.Count();
            }

            return accounts;
        }
    }
}
