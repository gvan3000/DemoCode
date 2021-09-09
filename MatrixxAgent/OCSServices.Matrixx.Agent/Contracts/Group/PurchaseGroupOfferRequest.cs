using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Group
{
    public class AddOfferToGroupRequest
    {
        public string ExternalId { get; set; }

        public string OfferCode { get; set; }

        public Dictionary<string, string> CustomPurchaseOfferConfigurationParameters { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
