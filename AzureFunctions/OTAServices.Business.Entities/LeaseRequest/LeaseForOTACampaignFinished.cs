
using System.Runtime.Serialization;

namespace OTAServices.Business.Entities.LeaseRequest
{
    [DataContract]
    public class LeaseForOTACampaignFinished
    {
        [DataMember]
        public string InstanceId { get; set; }
        [DataMember]
        public bool IsSuccedeed { get; set; }
    }
}
