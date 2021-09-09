using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class CancelOfferForGroupRequest
    {
        public string ObjectId { get; set; }

        public List<int> OfferIds { get; set; }
    }
}
