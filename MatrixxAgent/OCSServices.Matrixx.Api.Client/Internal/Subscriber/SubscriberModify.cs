using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Api.Client.Internal.Subscriber
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    internal class SubscriberModify : BaseApiOperation, IPostApiOperation<ModifySubscriberRequest, MatrixxResponse>
    {

        internal SubscriberModify(Endpoint endpoint) : base(endpoint)
        {
        }

        public Task<MatrixxResponse> Execute(ModifySubscriberRequest payLoad)
        {
            return Post<ModifySubscriberRequest, MatrixxResponse>(payLoad);
        }
    }
}
