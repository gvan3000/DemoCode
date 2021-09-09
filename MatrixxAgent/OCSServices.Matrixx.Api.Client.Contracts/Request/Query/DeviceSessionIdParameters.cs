using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Query
{
    [ApiMethodInfo(UrlTemplate = "/device/#DeviceId#/session")]
    public class DeviceSessionIdParameters : IQueryParameters
    {
        [UrlTemplateParameter(Name = "DeviceId")]
        public string DeviceId { get; private set; }

        public DeviceSessionIdParameters(string deviceId)
        {
            DeviceId = deviceId;
        }
    }
}
