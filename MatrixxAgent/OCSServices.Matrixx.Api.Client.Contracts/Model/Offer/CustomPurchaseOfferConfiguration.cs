using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class CustomPurchaseOfferConfiguration : MatrixxObject
    {
        public CustomPurchaseOfferConfiguration()
        {
            Configuration = new Dictionary<string, string>();
        }

        [MatrixxContractMember]
        public Dictionary<string, string> Configuration { get; set; }
    }
}
