using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OTAServices.Business.Entities.OTACampaignSubscribers
{
    [DataContract]
    public class OasisRequest
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int CampaignId { get; set; }
        [DataMember]
        public Guid SubscriberListId { get; set; }
        [DataMember]
        public int TargetSimProfileId { get; set; }
        [DataMember]
        public string Iccid { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Notes { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }

        [DataMember]
        public string ImsiInfo { get; set; }
        [DataMember]
        public string ProductInfo { get; set; }
        [DataMember]
        public string ProvisioningDataInfo { get; set; }
        [DataMember]
        public int? OriginalSimProfileId { get; set; }
    }
}
