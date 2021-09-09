using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Offer;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response.Offer
{
    [MatrixxContract(Name = "MtxResponsePurchase")]
    public class AddOfferToSubscriberResponse : MatrixxObject
    {
        [MatrixxContractMember(Name = "ObjectId")]
        public string ObjectId { get; set; }

        [MatrixxContractMember(Name = "DeviceIdArray")]
        public StringValueCollection DeviceList { get; set; }

        [MatrixxContractMember(Name = "PurchaseInfoArray")]
        public PurchaseInfoCollection PurchaseInfoList { get; set; }

        [MatrixxContractMember(Name = "Result")]
        public int? Result { get; set; }

        [MatrixxContractMember(Name = "ResultText")]
        public string ResultText { get; set; }

        [MatrixxContractMember(Name = "ResourceIdArray")]
        public IntegerValueCollection ResourceIdList { get; set; }
    }
}
