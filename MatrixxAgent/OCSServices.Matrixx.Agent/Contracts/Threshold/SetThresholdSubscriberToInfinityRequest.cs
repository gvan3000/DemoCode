using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Agent.Contracts.Threshold
{
    public class SetThresholdSubscriberToInfinityRequest
    {
        public Guid CrmProductId { get; set; }
        public int ResourceId { get; set; }
        public int ThresholdId { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
