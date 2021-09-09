using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Results
{
    public class PurchaseOfferResult  : BaseResult 
    {
         public List<PurchaseInfo> PurchaseInfos { get; set; }
    }
}
