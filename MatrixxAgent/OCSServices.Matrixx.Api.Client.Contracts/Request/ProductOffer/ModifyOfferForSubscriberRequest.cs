using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer
{
    [ApiMethodInfo(UrlTemplate = "/subscriber")]
    [MatrixxContract(Name = "MtxRequestSubscriberModifyOffer")]
    public class ModifyOfferForSubscriberRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "SubscriberSearchData")]
        public SubscriberSearchData SearchData { get; set; }

        [MatrixxContractMember(Name = "ResourceId")]
        public int OfferResourceId { get; set; }

        [MatrixxContractMember(Name = "EndTime")]
        public DateTime? EndTime { get; set; }

        [MatrixxContractMember(Name = "StartTime")]
        public DateTime? StartTime { get; set; }
    }
}
