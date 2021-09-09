using System;
using System.Runtime.Serialization;

namespace OTAServices.Business.Entities.OTACampaign
{
    [DataContract]
    public class Campaign
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public int IccidCount { get; set; }
        [DataMember]
        public int OriginalSimProfile { get; set; }
        [DataMember]
        public int TargetSimProfile { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public bool IsEndOfCampaignNotificationSent { get; set; }

    }
}
