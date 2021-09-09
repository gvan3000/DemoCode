using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Agent.Contracts.Offer
{
    public class ModifyOfferForSubscriberRequest
    {
        public Guid ProductId { get; set; }
        public int OfferResourceId { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? StartTime { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
