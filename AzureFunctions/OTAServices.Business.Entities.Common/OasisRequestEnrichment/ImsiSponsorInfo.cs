using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OTAServices.Business.Entities.Common.OasisRequestEnrichment
{
    [DataContract]
    public class ImsiSponsorInfo
    {
        [DataMember]
        public string Iccid { get; set; }
        [DataMember]
        public string Imsi { get; set; }
        [DataMember]
        public string SponsorName { get; set; }
        [DataMember]
        public string ImsiIndex { get; set; }
        [DataMember]
        public string MccList { get; set; }
        [DataMember]
        public ImsiOperation ImsiOperation { get; set; }
     }
}
