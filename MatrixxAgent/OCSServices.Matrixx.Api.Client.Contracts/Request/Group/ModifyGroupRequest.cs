using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Group;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Group
{
    [ApiMethodInfo(UrlTemplate = "/group")]
    [MatrixxContract(Name = "MtxRequestGroupModify")]
    public class ModifyGroupRequest : MatrixxObject
    {
        [UrlTemplateParameter(Name = "ObjectId")]
        public string ObjectId { get; set; }

        [MatrixxContractMember(Name = "GroupSearchData")]
        public Search.GroupSearchData GroupSearchData { get; set; }

        [MatrixxContractMember(Name = "ExternalId")]
        public string ExternalId { get; set; }

        [MatrixxContractMember(Name = "Name")]
        public string Name { get; set; }

        [MatrixxContractMember(Name = "Tier")]
        public string TierName { get; set; }

        [MatrixxContractMember(Name = "BillingCycle")]
        public BillingCycleCollection BillingCycleCollection { get; set; }
    }
}
