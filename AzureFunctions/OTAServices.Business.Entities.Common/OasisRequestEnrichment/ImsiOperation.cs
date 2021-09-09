using System.Runtime.Serialization;

namespace OTAServices.Business.Entities.Common.OasisRequestEnrichment
{
    [DataContract]
    public enum ImsiOperation
    {
        [DataMember]
        ADD,
        [DataMember]
        DELETE,
        [DataMember]
        UPDATE
    }
}
