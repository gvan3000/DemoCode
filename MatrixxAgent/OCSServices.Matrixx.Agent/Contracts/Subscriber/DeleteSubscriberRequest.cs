using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Subscriber
{
    public class DeactiveSubscriberRequest
    {
        public Guid CrmProductId { get; set; }
        public List<string> Imsis { get; set; }
        public int Status { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
