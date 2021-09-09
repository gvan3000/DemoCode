using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Offer;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Group
{
    [ApiMethodInfo(UrlTemplate = "/group")]
    [MatrixxContract(Name = "MtxRequestGroupPurchaseOffer")]
    public class PurchaseGroupOfferRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "GroupSearchData")]
        public Search.GroupSearchData GroupSearchData { get; set; }

        [MatrixxContractMember(Name = "OfferRequestArray")]
        public OfferRequestArray PurchaseInfoList { get; set; }
    }
}
