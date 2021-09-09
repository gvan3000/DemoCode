using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Offer
{
    public class GetPurchasedOffersRequest
    {
        public string MsIsdn { get; set; }

        public Guid? ProductId { get; set; }
    }
}