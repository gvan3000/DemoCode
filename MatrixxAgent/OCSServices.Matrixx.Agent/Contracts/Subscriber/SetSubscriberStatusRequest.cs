using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Subscriber
{
    public class SetSubscriberStatusRequest
    {
        public Guid CrmProductId { get; set; }
        public int Status { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
