using System;
using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class ActivateSubscriberAndPurchaseOfferRequest
    {
        public Guid CrmProductId { get; set; }
        public string Msisdn { get; set; }

        public List<string> Offers { get; set; }

        public SubscriberConfiguration SubscriberConfiguration { get; set; }

        public ActivateSubscriberAndPurchaseOfferRequest()
        {
            SubscriberConfiguration = new SubscriberConfiguration();
        }
    }

    public class SubscriberConfiguration
    {
        public bool BarAll { get; set; }
        public bool AntiShock { get; set; }
        public bool BarOffnet { get; set; }
        public bool BarInt { get; set; }
        public bool BarPremiumINFO { get; set; }
        public bool BarPremiumSMS { get; set; }
        public bool BarPremium { get; set; }
        public string ContractType { get; set; }
        public int ContractDuration { get; set; }
        public int SMSBundleType { get; set; }
        public int DataBundleType { get; set; }
        public bool RestrictToMVNO { get; set; }
        public bool BAOIntCalls { get; set; }
        public bool BAOSMS { get; set; }
        public bool BAISMS { get; set; }
        public bool BAIC { get; set; }
        public bool BAORCallexHC { get; set; }
    }
}
