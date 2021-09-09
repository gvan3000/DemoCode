using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Group
{
    public class CreateGroupAdminRequest
    {
        public Guid BusinessUnitId { get; set; }

        public string GroupCode { get; set; }

        public int SubscriberCreateStatus { get; set; }

        public Endpoint Endpoint { get; set; }
    }
}
