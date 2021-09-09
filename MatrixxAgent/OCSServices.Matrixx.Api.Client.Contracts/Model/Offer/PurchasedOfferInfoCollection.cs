using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class PurchasedOfferInfoCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxPurchasedOfferInfo")]
        public List<PurchasedOfferInfo> Values { get; set; }

        public PurchasedOfferInfoCollection()
        {
            Values = new List<PurchasedOfferInfo>();
        }
    }
}