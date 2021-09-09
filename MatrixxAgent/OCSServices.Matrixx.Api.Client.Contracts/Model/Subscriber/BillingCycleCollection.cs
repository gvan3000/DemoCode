using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber
{
    public class BillingCycleCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxBillingCycleData")]
        public BillingCycle BillingCycle { get; set; }
    }
}
