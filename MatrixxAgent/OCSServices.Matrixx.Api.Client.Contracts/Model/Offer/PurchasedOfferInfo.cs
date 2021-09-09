using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class PurchasedOfferInfo : MatrixxObject
    {
        [MatrixxContractMember(Name = "ProductOfferId")]
        public int ProductOfferId { get; set; }

        [MatrixxContractMember(Name = "ExternalId")]
        public string ProductOfferExternalId { get; set; }

        [MatrixxContractMember(Name = "ResourceId")]
        public int ResourceId { get; set; }

        [MatrixxContractMember(Name = "PurchaseTime")]
        public DateTime PurchaseTime { get; set; }

        [MatrixxContractMember(Name = "Status")]
        public int? Status { get; set; }

        [MatrixxContractMember(Name = "ProductOfferVersion")]
        public int ProductOfferVersion { get; set; }

        [MatrixxContractMember(Name = "EndTime")]
        public DateTime? EndTime { get; set; }
    }
}