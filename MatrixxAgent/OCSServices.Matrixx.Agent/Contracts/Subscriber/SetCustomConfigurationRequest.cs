using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Subscriber
{
    public class SetCustomConfigurationRequest
    {
        public Guid CrmProductId { get; set; }
        public Dictionary<string, string> CustomConfigurationParameters { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
