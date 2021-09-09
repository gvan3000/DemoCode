using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.ProductOffer
{
    [ApiMethodInfo(UrlTemplate = "/subscriber/#ObjectId#/offers/#ResourceIdArray#")]
    [MatrixxContract(Name = "MtxRequestSubscriberCancelOffer")]
    public class CancelOfferForSubscriberRequest : IQueryParameters
    {
        [UrlTemplateParameter(Name = "ObjectId")]
        public string ObjectId { get; set; }

        [UrlTemplateParameter(Name = "ResourceIdArray")]
        public string ResourceIdArray { get; set; }
    }
}
