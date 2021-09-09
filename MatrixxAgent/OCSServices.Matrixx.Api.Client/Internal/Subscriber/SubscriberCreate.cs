using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Internal.Subscriber
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    internal class SubscriberCreate : BaseApiOperation, IPostApiOperation<CreateSubscriberRequest, CreateObjectResponse>
    {
        internal SubscriberCreate(string baseUrl) : base(baseUrl)
        {
        }

        internal SubscriberCreate() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }

        public Task<CreateObjectResponse> Execute(CreateSubscriberRequest payLoad)
        {
            return Post<CreateSubscriberRequest, CreateObjectResponse>(payLoad);
        }
    }

}
