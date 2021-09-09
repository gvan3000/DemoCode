using System;

namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class PurchaseOfferRequest
    {
        public Guid ProductId { get; set; }
        public string PlanId { get; set; }
        public Guid TransactionId { get; set; }
    }
}
