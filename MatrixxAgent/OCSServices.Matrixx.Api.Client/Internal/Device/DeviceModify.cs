using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.Device
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    internal class DeviceModify : BaseApiOperation, IPostApiOperation<DeviceModifyRequest, MatrixxResponse>
    {
        public DeviceModify(Endpoint endpoint) : base(endpoint)
        {

        }

        public Task<MatrixxResponse> Execute(DeviceModifyRequest payLoad)
        {
            return Post<DeviceModifyRequest, MatrixxResponse>(payLoad);
        }
    }
}
