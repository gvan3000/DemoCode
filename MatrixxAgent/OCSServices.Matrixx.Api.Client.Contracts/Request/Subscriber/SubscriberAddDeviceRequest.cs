using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber
{
    [ApiMethodInfo(UrlTemplate = "/subscriber")]
    [MatrixxContract(Name = "MtxRequestSubscriberAddDevice")]
    public class SubscriberAddDeviceRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "SubscriberSearchData")]
        public SubscriberSearchData SubscriberSearchData { get; set; }

        [MatrixxContractMember(Name = "DeviceSearchData")]
        public DeviceSearchData DeviceSearchData { get; set; }
    }
}
