using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Device;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Device
{

    [ApiMethodInfo(UrlTemplate = "/device")]
    [MatrixxContract(Name = "MtxRequestDeviceModify")]
    public class DeviceModifyRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "DeviceSearchData")]
        public DeviceSearchData DeviceSearchData { get; set; }

        [MatrixxContractMember(Name = "Attr")]
        public MobileDeviceExtensionCollection MobileDeviceExtensionCollection { get; set; }

        [MatrixxContractMember(Name = "Status")]
        public int? Status { get; set; }
    }
}
