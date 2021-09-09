using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response.Device
{
    [MatrixxContract(Name= "MtxResponseDeviceSession")]
    public class ResponseDeviceSession : MatrixxObject
    {
        /// <summary>
        /// Charging sessions
        /// </summary>
        [MatrixxContractMember(Name = "ChargingSessionArray")]
        public ChargingSessionCollection ChargingSessionCollection { get; set; }

        /// <summary>
        /// Policy sessions
        /// </summary>
        [MatrixxContractMember(Name = "PolicySessionArray")]
        public PolicySessionCollection PolicySessionArray { get; set; }

        /// <summary>
        /// Contains error code: 0 -> SUCCESS 
        /// </summary>
        [MatrixxContractMember(Name = "Result")]       
        public int Result { get; set; }

        /// <summary>
        /// Human-readable description of the result
        /// </summary>
        [MatrixxContractMember(Name = "ResultText")]
        public string ResultText { get; set; }
    }
}
