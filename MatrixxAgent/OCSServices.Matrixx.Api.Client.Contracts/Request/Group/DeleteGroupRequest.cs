using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Group
{
    [ApiMethodInfo(UrlTemplate = "/group")]
    [MatrixxContract(Name = "MtxRequestGroupDelete")]
    public class DeleteGroupRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "GroupSearchData")]
        public Search.GroupSearchData GroupSearchData { get; set; }
    }
}