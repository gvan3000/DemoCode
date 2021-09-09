using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Agent.Contracts.Balance
{
    public class FlexTopupRequest
    {
        public Guid? ProductId { get; set; }

        public string Msisdn { get; set; }
        public decimal Amount { get; set; }
        public DateTime? EndTime { get; set; }
        public string Reason { get; set; }
        public int TemplateId { get; set; }
        public string BalanceType { get; set; }
        public bool PurchaseOffer { get; set; }
        public string OfferName { get; set; }
        public bool IsQuota { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
