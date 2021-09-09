using System;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts
{
    public class GetProductImeiRequest
    {
        public Guid BusinessUnitId { get; set; }
        public string Iccid { get; set; }
        public string Imei { get; set; }
        public bool IncludeChildren { get; set; }
    }
}