using System.Runtime.Serialization;

namespace OTAServices.Business.Entities.Common.SubscriberListLeaseRequestEnrichment
{
    [DataContract]
    public class LeaseRequest
    {
        [DataMember]
        public string Uiccid { get; set; }
        [DataMember]
        public string SponsorPrefix { get; set; }
    }
}
