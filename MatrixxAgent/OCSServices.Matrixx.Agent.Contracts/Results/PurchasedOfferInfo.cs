using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Results
{
    public class PurchasedOfferInfo
    {
        public string ProductOfferId { get; set; }

        public string ProductOfferExternalId { get; set; }

        public int ResourceId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime PurchaseTime { get; set; }

        public DateTime? CancelTime { get; set; }

        public string Status { get; set; }

        public string ProductOfferVersion { get; set; }
    }
}
