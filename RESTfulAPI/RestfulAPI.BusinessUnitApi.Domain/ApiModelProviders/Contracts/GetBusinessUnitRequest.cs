using System;

namespace RestfulAPI.BusinessUnitApi.Domain.ApiModelProviders.Contracts
{
    public class GetBusinessUnitRequest
    {
        public Guid UserCompanyId { get; set; }

        public Guid? UserBusinessUnitId { get; set; }

        public Guid? FilterBusinessUnitId { get; set; }

        public string FilterBusinessUnitName { get; set; }

        public string FilterCustomerId { get; set; }

        public string FilterHasSharedWallet { get; set; }

        public bool? FilterHasEndUserSubscripion { get; set; }

        public bool IncludeChildren { get; set; }
    }
}
