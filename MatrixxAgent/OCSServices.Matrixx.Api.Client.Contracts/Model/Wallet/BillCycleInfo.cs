using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Wallet
{
    public class BillCycleInfo : MatrixxObject
    {
        [MatrixxContractMember(Name = "BillingCycleId")]
        public int BillingCycleId { get; set; }

        [MatrixxContractMember(Name = "Period")]
        public int Period { get; set; }

        [MatrixxContractMember(Name = "PeriodInterval")]
        public int PeriodInterval { get; set; }

        [MatrixxContractMember(Name = "DatePolicy")]
        public int DatePolicy { get; set; }

        [MatrixxContractMember(Name = "DateOffset")]
        public int DateOffset { get; set; }

        [MatrixxContractMember(Name = "CurrentPeriodStartTime")]
        public DateTime? CurrentPeriodStartTime { get; set; }

        [MatrixxContractMember(Name = "CurrentPeriodEndTime")]
        public DateTime? CurrentPeriodEndTime { get; set; }

        [MatrixxContractMember(Name = "CurrentPeriodDuration")]
        public int CurrentPeriodDuration { get; set; }

        [MatrixxContractMember(Name = "CurrentPeriodOffset")]
        public int? CurrentPeriodOffset { get; set; }
    }
}
