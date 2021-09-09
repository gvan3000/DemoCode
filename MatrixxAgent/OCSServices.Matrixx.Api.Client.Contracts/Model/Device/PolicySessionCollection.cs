using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Device
{
    public class PolicySessionCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxPolicySessionInfo")]
        public List<PolicySessionInfo> Values { get; set; }
        public PolicySessionCollection()
        {
            Values = new List<PolicySessionInfo>();
        }
    }
}
