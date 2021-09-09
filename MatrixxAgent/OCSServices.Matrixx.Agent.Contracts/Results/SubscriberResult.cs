using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Results
{
    public class SubscriberResult : BaseResult
    {
        public string ObjectId { get; set; }
        public string ExternalId { get; set; }
        public List<string> DeviceIdArray { get; set; }
        public string Status { get; set; }
        public string StatusDescription { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string NotificationPreference { get; set; }
        public List<string> ParentGroupIdArray { get; set; }
        public CustomSubscriberConfigurationExtension CustomSubscriberConfigurationExtension { get;set;}
        public MobileDeviceExtension MobileDeviceExtension { get; set; }
        public List<PurchasedOfferInfo> PurchasedOfferArray { get; set; }
        public List<BalanceInfo> BalanceArray { get; set; }
    }
}