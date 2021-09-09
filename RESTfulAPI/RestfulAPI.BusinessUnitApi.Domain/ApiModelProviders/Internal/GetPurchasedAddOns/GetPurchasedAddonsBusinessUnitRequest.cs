using System;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetPurchasedAddOns
{
    public class GetPurchasedAddonsBusinessUnitRequest
    {
        public Guid? BusinessUnitId { get; set; }
        public bool IncludeExpired { get; set; }
    }
}
