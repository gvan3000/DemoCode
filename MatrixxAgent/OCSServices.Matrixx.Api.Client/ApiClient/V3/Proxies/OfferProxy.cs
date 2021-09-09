using OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;
using OCSServices.Matrixx.Api.Client.Contracts.Response;
using OCSServices.Matrixx.Api.Client.Contracts.Response.Offer;
using OCSServices.Matrixx.Api.Client.Internal.Group;
using OCSServices.Matrixx.Api.Client.Internal.ProductOffer;
using OCSServices.Matrixx.Api.Client.Internal.Subscriber;
using SplitProvisioning.Base.Data;
using System;
using System.Threading.Tasks;

namespace OCSServices.Matrixx.Api.Client.ApiClient.V3.Proxies
{
    public interface IOfferProxy : IDisposable
    {
        Task<AddOfferToSubscriberResponse> AddOfferToSubscriber(PurchaseOfferForSubscriberRequest request, Endpoint endpoint);
        Task<MatrixxResponse> CancelOfferForSubscriber(CancelOfferForSubscriberRequest request, Endpoint endpoint);
        Task<MatrixxResponse> CancelOfferForGroup(CancelOfferForGroupRequest request);

        Task<MatrixxResponse> ModifyOfferForSubscriber(ModifyOfferForSubscriberRequest request, Endpoint endpoint);

        Task<MatrixxResponse> ModifyOfferForGroup(ModifyOfferForGroupRequest request, Endpoint endpoint);
    }

    public class OfferProxy : BaseProxy, IOfferProxy
    {

        public Task<AddOfferToSubscriberResponse> AddOfferToSubscriber(PurchaseOfferForSubscriberRequest request, Endpoint endpoint)
        {
            using (var purchaseOfferQuery = new SubscriberPurchaseOffer(endpoint))
            {
                return purchaseOfferQuery.Execute(request);
            }
        }


        public Task<MatrixxResponse> CancelOfferForSubscriber(CancelOfferForSubscriberRequest request, Endpoint endpoint)
        {
            using (var cancelOfferQuery = new SubscriberCancelOffer(endpoint))
            {
                return cancelOfferQuery.Execute(request);
            }
        }

        public Task<MatrixxResponse> CancelOfferForGroup(CancelOfferForGroupRequest request)
        {
            using (var cancelOfferQuery = new GroupCancelOffer())
            {
                return cancelOfferQuery.Execute(request);
            }
        }
      

        public Task<MatrixxResponse> ModifyOfferForSubscriber(ModifyOfferForSubscriberRequest request, Endpoint endpoint)
        {
            using (var modifyOfferQuery = new SubscriberModifyOffer(endpoint))
            {
                return modifyOfferQuery.Execute(request);
            }
        }
     

        public Task<MatrixxResponse> ModifyOfferForGroup(ModifyOfferForGroupRequest request, Endpoint endpoint)
        {
            using (var modifyOfferQuery = new GroupModifyOffer(endpoint))
            {
                return modifyOfferQuery.Execute(request);
            }
        }
    }
}
