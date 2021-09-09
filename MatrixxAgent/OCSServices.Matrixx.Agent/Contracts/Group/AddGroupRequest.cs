using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Group
{
    public class AddGroupRequest
    {
        public string GroupCode { get; set; }

        public string ExternalId { get; set; }

        public string BillingCycleId { get; set; }

        public string AdminExternalId { get; set; }

        public string Tier { get; set; }

        public int Status { get; set; }

        public int? NotificationPreference { get; set; }

        public int? GroupReAuthPreference { get; set; }

        public int? BillCycleOffset { get; set; }

        public Endpoint Endpoint { get; set; }
    }
}
