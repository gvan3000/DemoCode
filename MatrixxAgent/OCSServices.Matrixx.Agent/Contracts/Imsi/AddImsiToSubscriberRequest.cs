using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Imsi
{
    public class AddImsiToSubscriberRequest
    {
        public Guid SubscriberExternalId { get; set; }
        public string NewImsi { get; set; }
        public List<string> MsisdnList { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
