using System;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Internal.GetAvailableMsisdn
{
    public class AvailableMsisdnProviderRequest
    {
        public Guid? BusinessUnitId { get; set; }
        public string QueryMsisdnStatus { get; set; }
        public string MsisdnStatus { get; set; }
        public bool IncludeChildren { get; set; }
        public string CountryCode { get; set; }
        public int? PerPage { get; set; }
        public int? Page { get; set; }
    }
}
