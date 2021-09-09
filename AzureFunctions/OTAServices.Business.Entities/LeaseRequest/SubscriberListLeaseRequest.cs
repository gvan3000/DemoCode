using OTAServices.Business.Entities.SimManagement;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OTAServices.Business.Entities.LeaseRequest
{
    [DataContract]
    public class SubscriberListLeaseRequest
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int CampaignId { get; set; }
        [DataMember]
        public Guid SubscriberListId { get; set; }
        [DataMember]
        public string LeaseRequests { get; set; }

    }
}
