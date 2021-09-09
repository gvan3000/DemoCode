using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;
using OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber;

namespace OCSServices.Matrixx.Api.Client.Contracts.Request.Subscriber
{
    [ApiMethodInfo(UrlTemplate = "/subscriber")]
    [MatrixxContract(Name = "MtxRequestSubscriberCreate")]
    public class CreateSubscriberRequest : MatrixxObject
    {
        [MatrixxContractMember(Name= "Status")]
        public int? Status { get; set; }

        [MatrixxContractMember(Name = "ContactPhoneNumber")]
        public string ContactPhoneNumber { get; set; }

        [MatrixxContractMember(Name = "ExternalId")]
        public string ExternalId { get; set; }

        [MatrixxContractMember(Name = "FirstName")]
        public string FirstName { get; set; }

        [MatrixxContractMember(Name = "LastName")]
        public string LastName { get; set; }

        [MatrixxContractMember(Name = "NotificationPreference")]
        public int? NotificationPreference { get; set; }

        [MatrixxContractMember(Name = "TimeZone")]
        public string TimeZone { get; set; }

        [MatrixxContractMember(Name = "Attr")]
        public CustomSubscriberConfigurationCollection CustomSubscriberConfigurations { get; set; }
    }
}
