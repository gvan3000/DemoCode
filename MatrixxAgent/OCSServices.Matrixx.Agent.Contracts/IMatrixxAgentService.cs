using OCSServices.Matrixx.Agent.Contracts.Requests;
using OCSServices.Matrixx.Agent.Contracts.Results;

namespace OCSServices.Matrixx.Agent.Contracts
{
    public interface IMatrixxAgentService
    {
        GetAllOffersResult GetAllOffers();
        PurchaseOfferResult PurchaseOffer(PurchaseOfferRequest request);
        SubscriberResult GetSubscriberByImsi(GetSubscriberByImsiRequest request);
        SubscriberResult GetSubscriberByProductId(GetSubscriberByProductIdRequest request);
        CancelPurchasedOfferResult CancelPurchasedOffer(CancelPurchasedOfferRequest request);
        AddSubscriberResult AddSubscriber(AddSubscriberRequest request);
        ActivateSubscriberAndPurchaseOfferResult ActivateSubscriberAndPurchaseOffer(ActivateSubscriberAndPurchaseOfferRequest activateSubscriberAndPurchaseOfferRequest);

        ModifySubscriberResult ModifySubscriber(ModifySubscriberRequest request);
    }
}