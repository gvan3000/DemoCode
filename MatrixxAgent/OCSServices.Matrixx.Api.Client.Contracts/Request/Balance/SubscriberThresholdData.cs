using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Balance
{
    [MatrixxContract(Name = "Threshold")]
    public class SubscriberThresholdData : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxThresholdData")]
        public ThresholdCollection ThresholdCollection { get; set; }
    }
}
