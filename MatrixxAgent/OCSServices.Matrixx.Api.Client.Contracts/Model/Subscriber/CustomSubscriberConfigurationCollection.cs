using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber
{
    public class CustomSubscriberConfigurationCollection : MatrixxObject
    {
        [MatrixxContractMember(Name = "CustSubscrCfgExtension")]
        public CustomSubscriberConfiguration CustomSubscriberConfiguration { get; set; }
    }
}