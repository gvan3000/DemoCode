using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class OfferRequestArray : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxPurchasedOfferData")]
        public List<PurchasedOfferData> Values { get; set; }

        public OfferRequestArray()
        {
            Values = new List<PurchasedOfferData>();
        }
    }
}
