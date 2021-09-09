using System.Runtime.Serialization;

namespace OTAServices.Business.Entities.Common.OasisRequestEnrichment
{
    [DataContract]
    public class ProvisioningDataInfo
    {
        [DataMember]
        public string Uiccid { get; set; }
        [DataMember]
        public string KIC { get; set; }
        [DataMember]
        public string KID { get; set; }
        [DataMember]
        public string KIK { get; set; }
        [DataMember]
        public string Scp80KeysetVersion { get; set; }
        [DataMember]
        public string PskKeysetVersion { get; set; }
        [DataMember]
        public string PskKeyIdentifier { get; set; }
        [DataMember]
        public string PskIdentity { get; set; }
        [DataMember]
        public string PskKey { get; set; }
    }
}
