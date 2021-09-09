using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber
{
    public class BillingCycle : MatrixxObject
    {
        [MatrixxContractMember(Name = "BillingCycleId")]
        public string BillingCycleId { get; set; }

        [MatrixxContractMember(Name = "DateOffset")]
        public int? DateOffset { get; set; }
    }

}
