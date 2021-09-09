using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Response;

namespace OCSServices.Matrixx.Api.Client.Internal.Subscriber
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    internal class SubscriberAddDevice : BaseApiOperation, IPostApiOperation<SubscriberAddDeviceRequest, MatrixxResponse>
    {
        internal SubscriberAddDevice(string baseUrl) : base(baseUrl)
        {
        }

        internal SubscriberAddDevice() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }

        public Task<MatrixxResponse> Execute(SubscriberAddDeviceRequest payLoad)
        {
            return Post<SubscriberAddDeviceRequest, MatrixxResponse>(payLoad);
        }
    }
}
