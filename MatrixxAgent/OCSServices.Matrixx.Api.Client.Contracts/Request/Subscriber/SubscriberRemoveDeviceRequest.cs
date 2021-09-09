using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Request.Search;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber
{
    [MatrixxContract(Name = "MtxRequestSubscriberRemoveDevice")]
    public class SubscriberRemoveDeviceRequest : MatrixxObject
    {
        [MatrixxContractMember(Name = "SubscriberSearchData")]
        public SubscriberSearchData SubscriberSearchData { get; set; }

        [MatrixxContractMember(Name = "DeviceSearchData")]
        public DeviceSearchData DeviceSearchData { get; set; }

        [MatrixxContractMember(Name = "DeleteSession")]
        public bool DeleteSession { get; set; }
    }
}
