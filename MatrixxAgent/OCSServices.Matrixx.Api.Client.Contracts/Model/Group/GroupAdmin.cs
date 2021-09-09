using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Group
{
    public class GroupAdmin : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxSubscriberSearchData")]
        public SearchCollection Subscriber { get; set; }
    }
}
