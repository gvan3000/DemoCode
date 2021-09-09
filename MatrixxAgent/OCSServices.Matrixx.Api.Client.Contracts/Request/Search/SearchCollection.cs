using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Search
{
    public class SearchCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "AccessNumber")]
        public string AccessNumber { get; set; }

        [MatrixxContractMember(Name = "ExternalId")]
        public string ExternalId { get; set; }

        [MatrixxContractMember(Name = "MultiRequestIndex")]
        public string MultiRequestIndex { get; set; }

        [MatrixxContractMember(Name = "Imsi")]
        public string Imsi { get; set; }
    }
}