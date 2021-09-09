using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Device
{
    [ApiMethodInfo(UrlTemplate = "/device")]
    [MatrixxContract(Name = "MtxRequestDeviceValidateSession")]
    public class ValidateDeviceSession : MatrixxObject
    {
        [MatrixxContractMember(Name = "SubscriberSearchData")]
        public DeviceSearchData SearchData { get; set; }

        [MatrixxContractMember(Name = "SessionType")]
        public int SessionType { get; set; }
    }
}
