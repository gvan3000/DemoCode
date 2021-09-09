using System.Collections.Generic;

namespace OCSServices.Matrixx.Agent.Contracts.Requests
{
    public class AddSubscriberRequest
    {
        public int Status { get; set; }
        public string ContactPhoneNumber { get; set; }
        public string SubscriberExternalId { get; set; }
        public string FirstName { get; set; }
        public List<string> Msisdns { get; set; }
        public List<string> Imsis { get; set; }
        public string MembershipExternalId { get; set; }
        public string BillingCycleId { get; set; }
    }
}