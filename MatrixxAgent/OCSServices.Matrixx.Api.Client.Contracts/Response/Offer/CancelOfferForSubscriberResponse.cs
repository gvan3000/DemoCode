using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Response.Offer
{
    [ApiMethodInfo(UrlTemplate = "/subscriber")]
    [MatrixxContract(Name = "MtxResponse")]
    public class CancelOfferForSubscriberResponse
    {
        [MatrixxContractMember(Name = "Result")]
        public int? Result { get; set; }

        [MatrixxContractMember(Name = "ResultText")]
        public string ResultText { get; set; }
    }
}
