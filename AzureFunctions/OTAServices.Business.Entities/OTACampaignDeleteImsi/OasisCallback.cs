using System;
using System.Runtime.Serialization;

namespace OTAServices.Business.Entities.OTACampaignDeleteImsi
{
    [DataContract]
    public class OasisCallback
    {
        [DataMember(Name = "action")]
        public string Action { get; set; }
        [DataMember(Name = "eid")]
        public string EId { get; set; }
        [DataMember(Name = "iccid")]
        public string Iccid { get; set; }
        [DataMember(Name = "tid")]
        public int TId { get; set; }
        [DataMember(Name = "timeEnd")]
        public DateTime TimeEnd { get; set; }
        [DataMember(Name = "timeStart")]
        public DateTime TimeStart { get; set; }
        [DataMember(Name = "status")]
        public string Status { get; set; }
    }
}
