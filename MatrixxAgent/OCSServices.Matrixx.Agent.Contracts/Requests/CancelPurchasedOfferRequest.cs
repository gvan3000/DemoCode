using System;

namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class CancelPurchasedOfferRequest
    {
        public Guid ProductId { get; set; }

        public string ProductOfferExternalId { get; set; }
    }
}