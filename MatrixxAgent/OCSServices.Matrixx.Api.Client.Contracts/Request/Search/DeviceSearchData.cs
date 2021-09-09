using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Search
{
    [MatrixxContract(Name = "DeviceSearchData")]
    public class DeviceSearchData : MatrixxObject
    {
        [MatrixxContractMember(Name = "MtxDeviceSearchData")]
        public SearchCollection SearchCollection { get; set; }
    }
}
