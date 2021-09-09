using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class PurchaseInfoCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxPurchaseInfo")]
        public List<PurchaseInfo> Values { get; set; }

        public PurchaseInfoCollection()
        {
            Values = new List<PurchaseInfo>();
        }
    }
}
