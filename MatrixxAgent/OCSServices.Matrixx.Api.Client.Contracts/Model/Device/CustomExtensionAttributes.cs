using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Device
{
    public class CustomExtensionAttributes : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxChargingSessionExtension")]
        public ChargingSessionExtension ChargingSessionExtension { get; set; }

    }
}
