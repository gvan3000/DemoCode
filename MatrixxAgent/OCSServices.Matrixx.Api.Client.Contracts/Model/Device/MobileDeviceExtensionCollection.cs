using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Device
{

    public class MobileDeviceExtensionCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxMobileDeviceExtension")]
        public MobileDeviceExtension MobileDeviceExtension { get; set; }
    }
}
