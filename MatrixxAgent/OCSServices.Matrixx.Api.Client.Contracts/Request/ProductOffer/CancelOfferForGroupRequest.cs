using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer
{
    [ApiMethodInfo(UrlTemplate = "/group/#ObjectId#/offers/#ResourceIdArray#")]
    [MatrixxContract(Name = "MtxRequestGroupCancelOffer")]
    public class CancelOfferForGroupRequest : IQueryParameters
    {
        [UrlTemplateParameter(Name = "ObjectId")]
        public string ObjectId { get; set; }

        [UrlTemplateParameter(Name = "ResourceIdArray")]
        public string ResourceIdArray { get; set; }
    }
}
