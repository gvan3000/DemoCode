using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber
{
    [ApiMethodInfo(UrlTemplate = "/subscriber")]
    [MatrixxContract(Name = "MtxRequestSubscriberModify")]
    public class ModifySubscriberRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "SubscriberSearchData")]
        public SubscriberSearchData SearchData { get; set; }

        [MatrixxContractMember(Name="Status")]
        public int? Status { get; set; }

        [MatrixxContractMember(Name = "Attr")]
        public CustomSubscriberConfigurationCollection CustomSubscriberConfigurations { get; set; }

        [MatrixxContractMember(Name = "BillingCycle")]
        public BillingCycleCollection BillingCycleConfigurations { get; set; }

        [MatrixxContractMember(Name = "FirstName")]
        public string FirstName { get; set; }

        [MatrixxContractMember(Name = "ContactPhoneNumber")]
        public string ContactPhoneNumber { get; set; }
    }
}
