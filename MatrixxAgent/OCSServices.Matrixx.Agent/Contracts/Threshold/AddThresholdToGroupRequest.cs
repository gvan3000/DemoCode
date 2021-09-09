using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Agent.Contracts.Threshold
{
    public class AddThresholdToGroupRequest
    {
        public Guid BusinessUnitId { get; set; }
        public string GroupCode { get; set; }
        public int ResourceId { get; set; }
        public int ThresholdId { get; set; }
        public decimal Amount { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
