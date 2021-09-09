using RestfulAPI.BusinessUnitApi.Domain.Models.BusinessUnitModels;
using System.Collections.Generic;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetBusinessUnit
{
    public class BusinessUnitModelEqualityComparer : IEqualityComparer<BusinessUnitModel>
    {
        public bool Equals(BusinessUnitModel x, BusinessUnitModel y)
        {
            return x != null && y != null && x.Id == y.Id;
        }
        public int GetHashCode(BusinessUnitModel obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
