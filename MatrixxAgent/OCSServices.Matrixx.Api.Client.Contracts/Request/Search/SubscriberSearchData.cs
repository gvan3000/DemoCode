using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Search
{
    [MatrixxContract(Name = "SubscriberSearchData")]
    public class SubscriberSearchData : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxSubscriberSearchData")]
        public SearchCollection SearchCollection { get; set; }
    }
}
