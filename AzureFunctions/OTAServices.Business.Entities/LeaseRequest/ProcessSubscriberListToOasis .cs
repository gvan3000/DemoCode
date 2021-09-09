using System;
using System.Runtime.Serialization;

namespace OTAServices.Business.Entities.LeaseRequest
{
    [DataContract]
    public class ProcessSubscriberListToOasis 
    {
        [DataMember]
        public Guid BusinessCaseId { get; set; }

        [DataMember]
        public Guid SubscriberListId { get; set; }

        [DataMember]
        public int CampaignId { get; set; }

        [DataMember]
        public int IccidCount { get; set; }

        [DataMember]
        public Guid ActionId { get; set; }
    }
}
