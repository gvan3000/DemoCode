using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer
{
    [ApiMethodInfo(UrlTemplate = "/group")]
    [MatrixxContract(Name = "MtxRequestGroupModifyOffer")]
    public class ModifyOfferForGroupRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "GroupSearchData")]
        public Search.GroupSearchData GroupSearchData { get; set; }

        [MatrixxContractMember(Name = "ResourceId")]
        public int OfferResourceId { get; set; }

        [MatrixxContractMember(Name = "EndTime")]
        public DateTime? EndTime { get; set; }

        [MatrixxContractMember(Name = "StartTime")]
        public DateTime? StartTime { get; set; }
    }
}
