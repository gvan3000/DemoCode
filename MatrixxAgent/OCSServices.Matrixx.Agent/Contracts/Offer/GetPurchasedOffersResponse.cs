using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Offer
{
    public class GetPurchasedOffersResponse
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public List<PurchaseOfferInfo> PurchasedOfferCollection { get; set; }
    }
}
