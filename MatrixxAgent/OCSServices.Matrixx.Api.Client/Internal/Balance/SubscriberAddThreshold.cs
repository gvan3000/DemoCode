using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Balance;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.Balance
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    public class SubscriberAddThreshold : BaseApiOperation, IPostApiOperation<SubscriberAddThresholdRequest, MatrixxResponse>
    {
        internal SubscriberAddThreshold(string baseUrl) : base(baseUrl)
        {
        }

        [Obsolete]
        internal SubscriberAddThreshold() : this(MatrixxApiConstants.DefaultBaseUrlKey)
        {
        }
        internal SubscriberAddThreshold(Endpoint endpoint) : base(endpoint)
        {
        }

        public Task<MatrixxResponse> Execute(SubscriberAddThresholdRequest payLoad)
        {
            return Post<SubscriberAddThresholdRequest, MatrixxResponse>(payLoad);
        }

        public Task<MatrixxResponse> Execute(SubscriberSetThresholdToInfinityRequest payLoad)
        {
            return Post<SubscriberSetThresholdToInfinityRequest, MatrixxResponse>(payLoad);
        }
    }
}
