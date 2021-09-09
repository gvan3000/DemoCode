using OCSServices.Matrixx.Agent.Contracts.Offer;
using OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using api = OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer;
using ModifyOfferForGroupRequest = OCSServices.Matrixx.Agent.Contracts.Offer.ModifyOfferForGroupRequest;
using ModifyOfferForSubscriberRequest = OCSServices.Matrixx.Agent.Contracts.Offer.ModifyOfferForSubscriberRequest;

namespace OCSServices.Matrixx.Agent.Business.Interfaces
{
    public interface IOffer
    {
        PurchaseOfferForSubscriberRequest BuildPurchaseOfferForSubscriberRequest(AddOfferToSubscriberRequest request);
        api.CancelOfferForSubscriberRequest BuildCancelOfferForSubscriberRequest(OCSServices.Matrixx.Agent.Contracts.Requests.CancelOfferForSubscriberRequest request);
        api.CancelOfferForGroupRequest BuildCancelOfferForGroupRequest(OCSServices.Matrixx.Agent.Contracts.Requests.CancelOfferForGroupRequest request);
        api.ModifyOfferForSubscriberRequest BuildModifyOfferForSubscriberRequest(ModifyOfferForSubscriberRequest request);
        api.ModifyOfferForGroupRequest BuildModifyOfferForGroupRequest(ModifyOfferForGroupRequest request);

    }
}
