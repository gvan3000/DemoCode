using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Agent.Contracts.Msisdn
{
    public class UpdateMsisdnListRequest
    {
        public List<string> NewMsIsdns { get; set; }
        public string PrimaryMsisdn { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
