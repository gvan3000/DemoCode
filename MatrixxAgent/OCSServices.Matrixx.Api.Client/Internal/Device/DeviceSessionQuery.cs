using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Device;
using SplitProvisioning.Base.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Internal.Device
{
    [ApiVersion(ApiVersion.v3, HttpMethod.GET)]
    public class DeviceSessionQuery : BaseApiOperation, IGetApiOperation<ResponseDeviceSession>
    {
        

        public DeviceSessionQuery(Endpoint endpoint) 
            : base(endpoint)
        {

        }

        public Task<ResponseDeviceSession> Execute(IQueryParameters parameters)
        {
            return Get<ResponseDeviceSession>(parameters);
        }
    }
}
