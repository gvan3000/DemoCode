using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Offer;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response.Device
{
    [MatrixxContract(Name = "MtxResponseDevice")]
    public class DeviceQueryResponse : MatrixxObject
    {
        [MatrixxContractMember(Name = "ObjectId")]
        public string ObjectId { get; set; }

        [MatrixxContractMember(Name = "ExternalId")]
        public string ExternalId { get; set; }

        [MatrixxContractMember(Name = "SubscriberId")]
        public string SubscriberId { get; set; }

        [MatrixxContractMember(Name = "Status")]
        public int Status { get; set; }

        [MatrixxContractMember(Name = "StatusDescription")]
        public string StatusDescription { get; set; }

        [MatrixxContractMember(Name = "DeviceType")]
        public int? DeviceType { get; set; }

        [MatrixxContractMember(Name = "Attr")]
        public MobileDeviceExtensionCollection MobileDeviceExtensionCollection { get; set; }

        [MatrixxContractMember(Name = "PurchasedOfferArray")]
        public PurchasedOfferInfoCollection PurchaseInfoList { get; set; }

        [MatrixxContractMember(Name = "Result")]
        public int? Result { get; set; }

        [MatrixxContractMember(Name = "ResultText")]
        public string ResultText { get; set; }
    }
}
