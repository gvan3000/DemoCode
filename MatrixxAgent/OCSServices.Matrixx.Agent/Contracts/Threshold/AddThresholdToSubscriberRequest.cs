using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Threshold
{
    public class AddThresholdToSubscriberRequest
    {
        public Guid CrmProductId { get; set; }
        public int ResourceId { get; set; }
        public int ThresholdId { get; set; }
        public decimal Amount { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
