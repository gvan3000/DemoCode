using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Balance
{
    [ApiMethodInfo(UrlTemplate = "/")]
    [MatrixxContract(Name = "MtxRequestSubscriberAddThreshold")]
    public class SubscriberSetThresholdToInfinityRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "SubscriberSearchData")]
        public SubscriberSearchData SearchData { get; set; }

        [MatrixxContractMember(Name = "BalanceResourceId")]
        public int BalanceResourceId { get; set; }

        [MatrixxContractMember(Name = "Threshold")]
        public ThresholdToInfinityData ThresholdData { get; set; }
    }
}
