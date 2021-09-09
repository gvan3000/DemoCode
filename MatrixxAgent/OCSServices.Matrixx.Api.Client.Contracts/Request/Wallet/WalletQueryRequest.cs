using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet
{
    [ApiMethodInfo(UrlTemplate = "/")]
    [MatrixxContract(Name = "MtxRequestSubscriberQueryWallet")]
    public class WalletQueryRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "SubscriberSearchData")]
        public SubscriberSearchData SearchData { get; set; }

    }
}
