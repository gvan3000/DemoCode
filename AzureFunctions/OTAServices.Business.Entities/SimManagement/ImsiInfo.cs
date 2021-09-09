using System.Runtime.Serialization;

namespace OTAServices.Business.Entities.SimManagement
{
    [DataContract]
    public class ImsiInfo
    {
        [DataMember]
        public string Iccid { get; set; }
        [DataMember]
        public long Imsi { get; set; }
        [DataMember]
        public string ImsiSponsorExternalId { get; set; }
        [DataMember]
        public string ImsiPrefix { get; set; }
        [DataMember]
        public bool IsFromOngoingCampaign { get; set; }
    }
}
