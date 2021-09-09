using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Device
{
    public class ContextArray : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxSessionContextInfo")]
        public List<SessionContextInfo> Values { get; set; }
        public ContextArray()
        {
            Values = new List<SessionContextInfo>();
        }
    }
}
