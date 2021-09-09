using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Offer
{
    public class CustomPurchaseOfferConfigurationCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "CustOfferCfgExtension")]
        public CustomPurchaseOfferConfiguration CustomPurchaseOfferConfiguration { get; set; }
    }
}
