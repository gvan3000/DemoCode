using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Subscriber;
using SplitProvisioning.Base.Data;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.Internal.Subscriber
{
    [ApiVersion(ApiVersion.v3, HttpMethod.GET)]
    internal class SubscriberQuery : BaseApiOperation, IGetApiOperation<SubscriberQueryResponse>
    {
        internal SubscriberQuery(string baseUrlKey) : base(baseUrlKey)
        {
        }

        internal SubscriberQuery() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }
        internal SubscriberQuery(Endpoint endpoint) : base(endpoint)
        {
        }

        public Task<SubscriberQueryResponse> Execute(IQueryParameters parameters)
        {
            return Get<SubscriberQueryResponse>(parameters);
        }
    }
}
