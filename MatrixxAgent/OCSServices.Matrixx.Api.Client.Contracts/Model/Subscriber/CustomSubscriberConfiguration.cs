using System.Collections.Generic;
using OCSServices.Matrixx.Api.Client.Contracts.Base;
using OCSServices.Matrixx.Api.Client.Contracts.Base.Attributes;

namespace OCSServices.Matrixx.Api.Client.Contracts.Model.Subscriber
{
    public class CustomSubscriberConfiguration : MatrixxObject
    {
        public CustomSubscriberConfiguration()
        {
            Configuration = new Dictionary<string, string>();
        }

        [MatrixxContractMember(Name = "ContractType")]
        public string ContractType { get; set; }

        [MatrixxContractMember]
        public Dictionary<string, string> Configuration { get; set; }
    }
}