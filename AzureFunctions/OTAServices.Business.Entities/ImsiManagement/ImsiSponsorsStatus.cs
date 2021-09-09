using System.Runtime.Serialization;

namespace OTAServices.Business.Entities.ImsiManagement
{
    [DataContract]
    public class ImsiSponsorsStatus
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public string ExternalId { get; set; }
        [DataMember]
        public int ImsiCount { get; set; }
    }
}
