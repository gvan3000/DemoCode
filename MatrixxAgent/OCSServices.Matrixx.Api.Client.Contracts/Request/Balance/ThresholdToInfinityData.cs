using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Balance
{
    [MatrixxContract(Name = "Threshold")]
    public class ThresholdToInfinityData : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxThresholdData")]
        public ThresholdToInfinityCollection ThresholdCollection { get; set; }
    }
}
