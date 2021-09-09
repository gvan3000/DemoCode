using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class PurchasePricePlanInfo : MatrixxObject
    {
        [MatrixxContractMember(Name = "ExternalId")]
        public string ProductOfferExternalId { get; set; }

        [MatrixxContractMember(Name = "BundleAmount")]
        public int BundleAmount { get; set; }

        [MatrixxContractMember(Name = "RetailPricePlanID")]
        public int RetailPricePlanId { get; set; }

        [MatrixxContractMember(Name = "WholesalePricePlanID_1")]
        public int WholesalePricePlanId { get; set; }

        [MatrixxContractMember(Name = "QuotaBlackList")]
        public string QuotaBlackList { get; set; }

        [MatrixxContractMember(Name = "BundleWhiteList")]
        public string BundleWhiteList { get; set; }
    }
}