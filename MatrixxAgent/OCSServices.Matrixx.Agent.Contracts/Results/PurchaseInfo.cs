using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Results
{
    public class PurchaseInfo
    {
        public string ProductOfferId { get; set; }

        public string ProductOfferVersion { get; set; }

        public string ExternalId { get; set; }

        public string ResourceId { get; set; }

        public List<RequiredBalanceInfo> RequiredBalanceArray { get; set; }
    }
}
