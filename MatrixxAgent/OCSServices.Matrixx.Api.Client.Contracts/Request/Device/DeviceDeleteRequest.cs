using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Device
{
    [MatrixxContract(Name = "MtxRequestDeviceDelete")]
    public class DeviceDeleteRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "DeviceSearchData")]
        public DeviceSearchData DeviceSearchData { get; set; }
    }
}
