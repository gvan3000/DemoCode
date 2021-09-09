using RestfulAPI.TeleenaServiceReferences.AccountService;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit
{
    public class AccountEqualityComparer : IEqualityComparer<AccountContract>
    {
        public bool Equals(AccountContract x, AccountContract y)
        {
            return x != null && y != null && x.Id == y.Id && x.PlanId == y.PlanId;
        }

        public int GetHashCode(AccountContract obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
