using System;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Device;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Device
{
    [ApiMethodInfo(UrlTemplate = "/device")]
    [MatrixxContract(Name = "MtxRequestDeviceCreate")]
    public class CreateDeviceRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "ExternalId")]
        public Guid? ExternalId { get; set; }

        [MatrixxContractMember(Name = "DeviceType")]
        public int? DeviceType { get; set; }

        [MatrixxContractMember(Name = "Attr")]
        public MobileDeviceExtensionCollection MobileDeviceExtensions { get; set; }
    }
}
