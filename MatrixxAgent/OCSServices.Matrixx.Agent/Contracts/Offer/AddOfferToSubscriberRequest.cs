using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Offer
{
    public class AddOfferToSubscriberRequest
    {
        public Guid CrmProductId { get; set; }
        public List<string> OffersToBePurchased { get; set; }
        public Dictionary<string, string> CustomPurchaseOfferConfigurationParameters { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
