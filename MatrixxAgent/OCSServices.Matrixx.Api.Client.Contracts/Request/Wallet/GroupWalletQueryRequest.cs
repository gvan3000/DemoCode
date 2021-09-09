using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Wallet
{
    [ApiMethodInfo(UrlTemplate = "/")]
    [MatrixxContract(Name = "MtxRequestGroupQueryWallet")]
    public class GroupWalletQueryRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "GroupSearchData")]
        public GroupSearchData SearchData { get; set; }

    }
}
