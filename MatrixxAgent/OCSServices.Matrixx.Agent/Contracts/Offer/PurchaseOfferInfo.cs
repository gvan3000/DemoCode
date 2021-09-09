using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Offer
{
    public class PurchaseOfferInfo
    {
        public int ProductOfferId { get; set; }

        public string ProductOfferExternalId { get; set; }

        public int ResourceId { get; set; }

        public DateTime PurchaseTime { get; set; }

        public int? Status { get; set; }

        public int ProductOfferVersion { get; set; }
    }
}