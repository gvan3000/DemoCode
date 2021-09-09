using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Subscriber
{
    public class CreateSubscriberRequest
    {
        public int CreateWithStatus { get; set; }
        public Guid CrmProductId { get; set; }

        public List<string> MsisdnList { get; set; }

        public List<string> ImsiList { get; set; }
        public List<string> MembershipCodes { get; set; }
        public string BillingCycleId { get; set; }
        public string GroupCode { get; set; }
        public int? BillingDateOffset { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
