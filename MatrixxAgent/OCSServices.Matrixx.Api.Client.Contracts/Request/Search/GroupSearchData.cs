using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Search
{
    [MatrixxContract(Name = "GroupSearchData")]
    public class GroupSearchData : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxGroupSearchData")]
        public SearchCollection SearchCollection { get; set; }
    }
}
