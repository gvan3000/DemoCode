using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Device
{
    public class QueryDeviceSessions
    {
        public string DeviceId { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
