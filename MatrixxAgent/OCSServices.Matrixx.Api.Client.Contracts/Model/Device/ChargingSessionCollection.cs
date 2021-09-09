using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Device
{
    public class ChargingSessionCollection : MatrixxObject
    {
        [MatrixxContractMember(Name ="MtxChargingSessionInfo")]
        public List<ChargingSessionInfo> Values { get; set; }
        public ChargingSessionCollection()
        {
            Values = new List<ChargingSessionInfo>();
        }
    }
}
