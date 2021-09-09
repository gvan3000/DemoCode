using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class CancelOfferForSubscriberRequest
    {
        public string ObjectId { get; set; }

        public List<int> OfferIds { get; set; }
        public SplitProvisioning.Base.Data.Endpoint Endpoint { get; set; }
    }
}
