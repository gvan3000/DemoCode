using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Sim.Swap
{
    public class SwapSimRequest
    {
        public Guid CrmProductId { get; set; }

        public List<string> Imsis { get; set; }

        public List<string> OldImsis { get; set; }

        public Endpoint Endpoint { get; set; }
    }
}
