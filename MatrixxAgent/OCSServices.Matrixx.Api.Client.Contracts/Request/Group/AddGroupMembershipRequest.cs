using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Group
{
    [ApiMethodInfo(UrlTemplate = "/group")]
    [MatrixxContract(Name = "MtxRequestGroupAddMembership")]
    public class AddGroupMembershipRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "GroupSearchData")]
        public GroupSearchData GroupSearchData { get; set; }

        [MatrixxContractMember(Name = "SubscriberArray")]
        public SubscriberSearchArray GroupMembers { get; set; }
    }
}
