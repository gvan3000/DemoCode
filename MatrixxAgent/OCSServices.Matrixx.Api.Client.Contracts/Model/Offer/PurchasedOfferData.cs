using System;
using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class PurchasedOfferData : MatrixxObject
    {
        [MatrixxContractMember(Name = "ExternalId")]
        public string ExternalId { get; set; }

        [MatrixxContractMember(Name = "StartTime")]
        public DateTime? StartTime { get; set; }

        [MatrixxContractMember(Name = "EndTime")]
        public DateTime? EndTime { get; set; }

        [MatrixxContractMember(Name = "Attr")]
        public CustomPurchaseOfferConfigurationCollection CustomPurchaseOfferConfiguration { get; set; }
    }
}
