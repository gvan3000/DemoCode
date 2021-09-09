using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Balance
{
    [ApiMethodInfo(UrlTemplate = "/")]
    [MatrixxContract(Name = "MtxRequestGroupAddThreshold")]
    public class GroupAddThresholdRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "GroupSearchData")]
        public GroupSearchData GroupSearchData { get; set; }

        [MatrixxContractMember(Name = "BalanceResourceId")]
        public int BalanceResourceId { get; set; }

        [MatrixxContractMember(Name = "Threshold")]
        public SubscriberThresholdData ThresholdData { get; set; }
    }
}
