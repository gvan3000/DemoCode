using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Response;

namespace OCSServices.Matrixx.Api.Client.Internal.Device
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    internal class DeviceCreate : BaseApiOperation, IPostApiOperation<CreateDeviceRequest, CreateObjectResponse>
    {
        internal DeviceCreate(string baseUrl) : base(baseUrl)
        {
        }

        internal DeviceCreate() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }

        public Task<CreateObjectResponse> Execute(CreateDeviceRequest payLoad)
        {
            return Post<CreateDeviceRequest, CreateObjectResponse>(payLoad);
        }
    }
}
