using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Device;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.Device
{
    [ApiVersion(ApiVersion.v3, HttpMethod.GET)]
    internal class DeviceQuery : BaseApiOperation, IGetApiOperation<DeviceQueryResponse>
    {
       

        internal DeviceQuery(Endpoint endpoint) : base(endpoint)
        {

        }

        public async Task<DeviceQueryResponse> Execute(IQueryParameters parameters)
        {
            return await Get<DeviceQueryResponse>(parameters);
        }


    }
}
