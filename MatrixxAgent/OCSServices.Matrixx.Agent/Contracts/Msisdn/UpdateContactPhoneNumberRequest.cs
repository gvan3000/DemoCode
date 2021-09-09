using SplitProvisioning.Base.Data;
using System;

namespace OCSServices.Matrixx.Agent.Contracts.Msisdn
{
    public class UpdateContactPhoneNumberRequest
    {
        public Guid CrmProductId { get; set; }
        public string PrimaryMsisdn { get; set; }
        public Endpoint Endpoint { get; set; }
    }
}
