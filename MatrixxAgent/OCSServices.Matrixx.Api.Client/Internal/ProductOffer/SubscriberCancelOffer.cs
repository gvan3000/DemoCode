using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.ProductOffer
{
    [ApiVersion(ApiVersion.v3, HttpMethod.DELETE)]
    internal class SubscriberCancelOffer : BaseApiOperation, IPostApiOperation<CancelOfferForSubscriberRequest, MatrixxResponse>
    {
        internal SubscriberCancelOffer(Endpoint endpoint) : base(endpoint)
        {
        }

        public Task<MatrixxResponse> Execute(CancelOfferForSubscriberRequest payLoad)
        {
            return Delete<MatrixxResponse>(payLoad);
        }
    }
}
