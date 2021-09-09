using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Results
{
    public class GetAllOffersResult : BaseResult
    {
        public GetAllOffersResult()
        {
            MtxPricingOfferInfoList = new List<PricingOfferInfo>();
        }

        public List<PricingOfferInfo> MtxPricingOfferInfoList { get; set; }
    }
}
