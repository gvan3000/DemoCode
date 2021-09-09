using System;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.ApiClient.Base;
using OCSServices.Matrixx.Api.Client.ApiClient.V3;
using OCSServices.Matrixx.Api.Client.Base;
using OCSServices.Matrixx.Api.Client.Base.Enums;
using OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Offer;
using SplitProvisioning.Base.Data;

namespace OCSServices.Matrixx.Api.Client.Internal.ProductOffer
{
    [ApiVersion(ApiVersion.v3, HttpMethod.POST)]
    internal class SubscriberPurchaseOffer : BaseApiOperation, IPostApiOperation<PurchaseOfferForSubscriberRequest, AddOfferToSubscriberResponse>
    {
        internal SubscriberPurchaseOffer(Endpoint endpoint) : base(endpoint)
        {

        }

        public Task<AddOfferToSubscriberResponse> Execute(PurchaseOfferForSubscriberRequest payLoad)
        {
            return Post<PurchaseOfferForSubscriberRequest, AddOfferToSubscriberResponse>(payLoad);
        }
    }
}