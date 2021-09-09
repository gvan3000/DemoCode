using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Offer
{
    public class SetCustomPurchaseOfferConfigurationRequest
    {
        public Dictionary<string, string> CustomConfigurationParameters { get; set; }
    }
}
