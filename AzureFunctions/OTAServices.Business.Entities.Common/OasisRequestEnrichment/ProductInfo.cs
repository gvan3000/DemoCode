using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace OTAServices.Business.Entities.Common.OasisRequestEnrichment
{
    [DataContract]
    public class ProductInfo
    {
        [DataMember]
        public string Uiccid { get; set; }
        [DataMember]
        public string Msisdn { get; set; }
        [DataMember]
        public string PrimaryImsi { get; set; }
    }
}
